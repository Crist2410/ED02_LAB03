using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace LibreriaGenericos
{
    class Huffman
    {
        FileStream Archivo;
        FileStream ArchivoOriginal;
        int NumeroArchivos = 1;
        public void Comprimir(string RutaOriginal, string RutaDestino)
        {
            List<NodoHuff> ListaOriginal = new List<NodoHuff>();
            ColaPrioridad<NodoHuff> Cola = new ColaPrioridad<NodoHuff>();
            NodoHuff NodoAux;
            Nodo<NodoHuff> RaizArbol = new Nodo<NodoHuff>();
            string RutaArbol = Path.Combine(RutaDestino, "Archivo_Comp_" + NumeroArchivos++ + ".huff");
            Archivo = new FileStream(RutaArbol, FileMode.OpenOrCreate);
            ArchivoOriginal = new FileStream(RutaOriginal, FileMode.OpenOrCreate);
            int TotalRepeticiones = 0;
            using var reader = new BinaryReader(ArchivoOriginal);
            var buffer = new byte[100];
            Dictionary<byte, int> DicByte = new Dictionary<byte, int>();
            while (ArchivoOriginal.Position < ArchivoOriginal.Length)
            {
                buffer = reader.ReadBytes(100);
                foreach (var Item in buffer)
                {
                    if (DicByte.ContainsKey(Item))
                        DicByte[Item]++;
                    else
                        DicByte.Add(Item, 1);
                    ++TotalRepeticiones;
                }
            }
            foreach (var Item in DicByte)
            {
                NodoAux = new NodoHuff();
                NodoAux.ValorByte = Item.Key;
                NodoAux.Repeticiones = Item.Value;
                NodoAux.Probabilidad = (float)NodoAux.Repeticiones / TotalRepeticiones;
                ListaOriginal.Add(NodoAux);
                Cola.Add(NodoAux, NodoAux.Comparar);
            }
            Dictionary<byte, string> DicString = RealizarArbol(RaizArbol, Cola, 1);
            ArchivoOriginal.Seek(0, SeekOrigin.Begin);
            MetaData(Archivo, ListaOriginal);
            string Binario = "";
            int Ceros = 0;
            byte[] Bytes;
            while (ArchivoOriginal.Position < ArchivoOriginal.Length)
            {
                int ContadorBuffer = 0;
                Bytes = new byte[100];
                buffer = reader.ReadBytes(100);
                foreach (var Item in buffer)
                    Binario += DicString[Item];
                for (int i = 0; i < Bytes.Length; i++)
                {
                    if (Binario.Length >= 8)
                    {
                        Bytes[i] = Convert.ToByte(Binario.Substring(0, 8), 2);
                        Binario = Binario.Substring(8);
                    }
                    else if (ArchivoOriginal.Position == ArchivoOriginal.Length && Binario != "")
                    {
                        Ceros = Binario.Length;
                        Bytes[i] = Convert.ToByte(Binario.Substring(0), 2);
                        Binario = "";
                    }
                    else
                        break;
                    ContadorBuffer++;
                }
                Archivo.Write(Bytes, 0, ContadorBuffer);
                Archivo.Flush();
            }
            Bytes = BitConverter.GetBytes(Ceros);
            Archivo.Seek(5, SeekOrigin.Begin);
            Archivo.Write(Bytes, 0, 2);
            Archivo.Flush();
            reader.Close();
            ArchivoOriginal.Close();
            Archivo.Close();
        }
        public void Descomprimir(string RutaOriginal, string RutaDestino)
        {
            ColaPrioridad<NodoHuff> Cola = new ColaPrioridad<NodoHuff>();
            List<NodoHuff> ListaAux = new List<NodoHuff>();
            byte[] DatosSize = new byte[4];
            byte[] Metadata;
            byte[] CerosBytes = new byte[4];
            string Binario = "";
            int TotalDatos = 0;
            int Ceros = 0;
            string RutaArbol = Path.Combine(RutaDestino, "Archivo_Des_" + NumeroArchivos++ + ".txt");
            Archivo = new FileStream(RutaArbol, FileMode.OpenOrCreate);
            ArchivoOriginal = new FileStream(RutaOriginal, FileMode.Open);
            using var reader = new BinaryReader(ArchivoOriginal);
            //Obtener Cantidad de Datos para la tabla
            ArchivoOriginal.Seek(0, SeekOrigin.Begin);
            ArchivoOriginal.Read(DatosSize, 0, 4);
            int Size = BitConverter.ToInt32(DatosSize, 0);
            int Total = Size / 5;
            Metadata = new byte[Size];
            //Creos 
            ArchivoOriginal.Seek(5, SeekOrigin.Begin);
            ArchivoOriginal.Read(CerosBytes, 0, 2);
            Ceros = BitConverter.ToInt32(CerosBytes, 0);
            // Realizar tabla
            ArchivoOriginal.Seek(8, SeekOrigin.Begin);
            ArchivoOriginal.Read(Metadata, 0, Size);
            for (int i = 0; i < Total; i++)
            {
                NodoHuff AuxNodo = new NodoHuff();
                byte[] ByteCantidad = new byte[4];
                for (int k = 1; k < 5; k++)
                    if (Metadata[(i * 5) + k] != 0)
                        ByteCantidad[k - 1] = Metadata[(i * 5) + k];
                AuxNodo.Repeticiones = BitConverter.ToInt32(ByteCantidad, 0);
                TotalDatos += AuxNodo.Repeticiones;
                AuxNodo.ValorByte = Metadata[i * 5];
                ListaAux.Add(AuxNodo);
            }
            for (int i = 0; i < Total; i++)
            {
                NodoHuff AuxNodo = ListaAux[i];
                AuxNodo.Probabilidad = (float)AuxNodo.Repeticiones / TotalDatos;
                Cola.Add(AuxNodo, AuxNodo.Comparar);
            }
            Nodo<NodoHuff> RaizArbol = new Nodo<NodoHuff>();
            Dictionary<byte, string> DicString = RealizarArbol(RaizArbol, Cola, 1);
            Dictionary<string, byte> DicByte = new Dictionary<string, byte>();
            foreach (var Item in DicString)
                DicByte.Add(Item.Value, Item.Key);
            var buffer = new byte[100];
            int ContadorBuffer = 0;
            string CodigoComparador = "";
            while (ArchivoOriginal.Position < ArchivoOriginal.Length)
            {
                int ContadorPosicion = 0;
                byte[] Bytes = new byte[250];
                buffer = reader.ReadBytes(100);
                foreach (var Item in buffer)
                {
                    ContadorPosicion++;
                    string TextoAux = Convert.ToString(Item, 2);
                    if (TextoAux.Length == 8)
                        Binario += TextoAux;
                    else if (ArchivoOriginal.Position == ArchivoOriginal.Length && ContadorPosicion == buffer.Length)
                        Binario += TextoAux.PadLeft(Ceros, '0');
                    else
                        Binario += TextoAux.PadLeft(8, '0');
                    while (Binario != "")
                    {
                        CodigoComparador += Binario.Substring(0, 1);
                        Binario = Binario.Substring(1);
                        if (DicByte.ContainsKey(CodigoComparador))
                        {
                            Bytes[ContadorBuffer] = DicByte[CodigoComparador];
                            CodigoComparador = "";
                            ContadorBuffer++;
                        }
                    }
                }
                Archivo.Write(Bytes, 0, ContadorBuffer);
                Archivo.Flush();
                ContadorBuffer = 0;
            }
        }
        void MetaData(FileStream Archivo, List<NodoHuff> ListaValores)
        {
            byte[] Metadatos = new byte[(ListaValores.Count * 5) + 8];
            byte[] ByteMeta = BitConverter.GetBytes(Metadatos.Length - 8);
            for (int i = 0; i < ByteMeta.Length; i++)
                Metadatos[i] = ByteMeta[i];
            for (int i = 0; i < ListaValores.Count; i++)
            {
                byte[] ByteCantidad = BitConverter.GetBytes(ListaValores[i].Repeticiones);
                Metadatos[(i * 5) + 8] = ListaValores[i].ValorByte; ;
                for (int k = 0; k < ByteCantidad.Length; k++)
                    Metadatos[(i * 5) + k + 9] = ByteCantidad[k];
            }
            Archivo.Write(Metadatos);
            Archivo.Flush();
        }
        Dictionary<byte, string> RealizarArbol(Nodo<NodoHuff> Raiz, ColaPrioridad<NodoHuff> Cola, int Nombre)
        {
            Dictionary<byte, string> DicCodigo = new Dictionary<byte, string>();
            List<Nodo<NodoHuff>> ListaArboles = new List<Nodo<NodoHuff>>();
            do
            {
                NodoHuff NodoHijo1 = new NodoHuff();
                NodoHuff NodoHijo2 = new NodoHuff();
                NodoHuff Padre = new NodoHuff();
                Nodo<NodoHuff> Izquierda = new Nodo<NodoHuff>();
                Nodo<NodoHuff> Derecha = new Nodo<NodoHuff>();
                Nodo<NodoHuff> AuxRaiz = new Nodo<NodoHuff>();
                NodoHijo1 = Cola.Delete(NodoHijo1.Comparar);
                foreach (var Item in ListaArboles)
                    if (Item.Valor.ID == NodoHijo1.ID)
                        Izquierda = Item;
                NodoHijo2 = Cola.Delete(NodoHijo2.Comparar);
                foreach (var Item in ListaArboles)
                    if (Item.Valor.ID == NodoHijo2.ID)
                        Derecha = Item;
                if (Izquierda.Valor == null)
                    Izquierda.Valor = NodoHijo1;
                else
                    ListaArboles.Remove(Izquierda);
                if (Derecha.Valor == null)
                    Derecha.Valor = NodoHijo2;
                else
                    ListaArboles.Remove(Derecha);
                Padre.ID = "Nodo" + Nombre;
                Padre.Probabilidad = NodoHijo2.Probabilidad + NodoHijo1.Probabilidad;
                Padre.Valor = Convert.ToChar(Nombre++);
                AuxRaiz.Valor = Padre;
                AuxRaiz.Izquierda = Izquierda;
                AuxRaiz.Derecha = Derecha;
                ListaArboles.Add(AuxRaiz);
                if (Cola.Get() != null)
                    Cola.Add(Padre, Padre.Comparar);
            } while (Cola.Get() != null);
            Raiz = ListaArboles[0];
            Recorrido(Raiz, DicCodigo);
            return DicCodigo;
        }
        void Recorrido(Nodo<NodoHuff> RaizActual, Dictionary<byte, string> Lista)
        {
            if (RaizActual.Izquierda == null)
                Lista.Add(RaizActual.Valor.ValorByte, RaizActual.Valor.Codigo);
            else
            {
                RaizActual.Izquierda.Valor.Codigo = RaizActual.Valor.Codigo + 0;
                Recorrido(RaizActual.Izquierda, Lista);
                RaizActual.Derecha.Valor.Codigo = RaizActual.Valor.Codigo + 1;
                Recorrido(RaizActual.Derecha, Lista);
            }
        }
    }
}
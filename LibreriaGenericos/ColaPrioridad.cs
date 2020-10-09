using System;
using System.Collections.Generic;
using System.Text;

namespace LibreriaGenericos
{
    public class ColaPrioridad<T>
    {
        Nodo<T> Raiz = new Nodo<T>();
        int TotalNodos;
        bool Incertado;
        public void Add(T Valor, Delegate Delegado)
        {
            TotalNodos++;
            Incertado = false;
            Insertar(Valor, Delegado, Raiz);
        }
        public T Get()
        {
            return Obtener();
        }
        public T Delete(Delegate Delegado)
        {
            Nodo<T> AuxNodo = new Nodo<T>() { Valor = Obtener() };
            if (TotalNodos == 1)
            {
                Raiz = new Nodo<T>();
                TotalNodos--;
            }
            else if (TotalNodos != 0)
            {
                Borrar(Raiz, 1);
                BorraBalanceo(Delegado, Raiz);
            }
            return AuxNodo.Valor;
        }
        private void Ordenamiento(Delegate Delegado, Nodo<T> NodoRaiz)
        {
            Nodo<T> AuxNodo = new Nodo<T>();
            AuxNodo.Valor = NodoRaiz.Valor;
            if (NodoRaiz.Izquierda.Valor != null && Convert.ToInt32(Delegado.DynamicInvoke(NodoRaiz.Izquierda.Valor, NodoRaiz.Valor)) == -1)
            {
                NodoRaiz.Valor = NodoRaiz.Izquierda.Valor;
                NodoRaiz.Izquierda.Valor = AuxNodo.Valor;
            }
            else if (NodoRaiz.Derecha.Valor != null && Convert.ToInt32(Delegado.DynamicInvoke(NodoRaiz.Derecha.Valor, NodoRaiz.Valor)) == -1)
            {
                NodoRaiz.Valor = NodoRaiz.Derecha.Valor;
                NodoRaiz.Derecha.Valor = AuxNodo.Valor;
            }
        }
        void Insertar(T Valor, Delegate Delegado, Nodo<T> NodoRaiz)
        {
            if (NodoRaiz.Valor == null)
            {
                NodoRaiz.Valor = Valor;
                NodoRaiz.Izquierda = new Nodo<T>();
                NodoRaiz.Derecha = new Nodo<T>();
                NodoRaiz.Posicion = TotalNodos;
                Incertado = true;
            }
            else if (NodoRaiz.Derecha.Valor != null && NodoRaiz.Izquierda.Valor != null)
            {
                Insertar(Valor, Delegado, NodoRaiz.Izquierda);
                if (!Incertado)
                    Insertar(Valor, Delegado, NodoRaiz.Derecha);
            }
            else if (NodoRaiz.Izquierda.Valor == null && (TotalNodos) / 2 == NodoRaiz.Posicion)
            {
                Insertar(Valor, Delegado, NodoRaiz.Izquierda);
            }
            else if (NodoRaiz.Derecha.Valor == null && (TotalNodos - 1) / 2 == NodoRaiz.Posicion)
            {
                Insertar(Valor, Delegado, NodoRaiz.Derecha);
            }
            if (NodoRaiz.Derecha.Valor != null || NodoRaiz.Izquierda.Valor != null)
                Ordenamiento(Delegado, NodoRaiz);
        }
        private int ObtenerNivel(int Inicio)
        {
            int Nivel = Convert.ToInt32(Math.Truncate(Math.Log(TotalNodos) / Math.Log(2)));
            int Valor = TotalNodos;
            for (int i = Inicio; i < Nivel; i++)
                Valor /= 2;
            return Valor;
        }
        private void BorraBalanceo(Delegate Delegado, Nodo<T> NodoRaiz)
        {
            Nodo<T> AuxNodo = new Nodo<T>();
            AuxNodo.Valor = NodoRaiz.Valor;
            if (NodoRaiz.Izquierda.Valor != null && NodoRaiz.Derecha.Valor != null &&
                Convert.ToInt32(Delegado.DynamicInvoke(NodoRaiz.Izquierda.Valor, NodoRaiz.Valor)) == -1 &&
                Convert.ToInt32(Delegado.DynamicInvoke(NodoRaiz.Izquierda.Valor, NodoRaiz.Derecha.Valor)) <= 0)
            {
                NodoRaiz.Valor = NodoRaiz.Izquierda.Valor;
                NodoRaiz.Izquierda.Valor = AuxNodo.Valor;
                BorraBalanceo(Delegado, NodoRaiz.Izquierda);
            }
            else if (NodoRaiz.Derecha.Valor != null && Convert.ToInt32(Delegado.DynamicInvoke(NodoRaiz.Derecha.Valor, NodoRaiz.Valor)) == -1)
            {
                NodoRaiz.Valor = NodoRaiz.Derecha.Valor;
                NodoRaiz.Derecha.Valor = AuxNodo.Valor;
                BorraBalanceo(Delegado, NodoRaiz.Derecha);
            }
            else if (NodoRaiz.Izquierda.Valor != null && Convert.ToInt32(Delegado.DynamicInvoke(NodoRaiz.Izquierda.Valor, NodoRaiz.Valor)) == -1)
            {
                NodoRaiz.Valor = NodoRaiz.Izquierda.Valor;
                NodoRaiz.Izquierda.Valor = AuxNodo.Valor;
                BorraBalanceo(Delegado, NodoRaiz.Izquierda);
            }
        }

        void Borrar(Nodo<T> NodoRaiz, int Inicio)
        {
            int Posicion = ObtenerNivel(Inicio);
            if (NodoRaiz.Posicion == TotalNodos)
            {
                if (TotalNodos == 1)
                    Raiz = new Nodo<T>();
                Raiz.Valor = NodoRaiz.Valor;
                TotalNodos--;
            }
            else if (NodoRaiz.Derecha.Posicion == Posicion)
            {
                Borrar(NodoRaiz.Derecha, ++Inicio);
                if (NodoRaiz.Derecha.Posicion == (TotalNodos + 1))
                    NodoRaiz.Derecha = new Nodo<T>();
            }
            else if (NodoRaiz.Izquierda.Posicion == Posicion)
            {
                Borrar(NodoRaiz.Izquierda, ++Inicio);
                if (NodoRaiz.Izquierda.Posicion == (TotalNodos + 1))
                    NodoRaiz.Izquierda = new Nodo<T>();
            }

        }

        T Obtener()
        {
            return Raiz.Valor;
        }
    }
}

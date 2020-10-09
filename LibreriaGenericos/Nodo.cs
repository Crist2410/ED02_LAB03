using System;
using System.Collections.Generic;
using System.Text;

namespace LibreriaGenericos
{
    class Nodo<T>
    {
        public Nodo<T> Izquierda { get; set; }
        public Nodo<T> Derecha { get; set; }
        public T Valor { get; set; }
        public int Posicion { get; set; }
    }
}

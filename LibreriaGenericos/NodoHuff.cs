using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;


namespace LibreriaGenericos
{
    class NodoHuff : IComparable<NodoHuff>
    {
       public byte ValorByte { get; set; }
        public string ID { get; set; }
        public char Valor { get; set; }
        public int Repeticiones { get; set; }
        public double Probabilidad { get; set; }
        public string Codigo { get; set; }

        public int CompareTo([AllowNull] NodoHuff other)
        {
            return Probabilidad.CompareTo(other.Probabilidad);
        }
        public Comparison<NodoHuff> Comparar = delegate (NodoHuff Far1, NodoHuff Far2)
        {
            return Far1.Probabilidad.CompareTo(Far2.Probabilidad);
        };
    }
}

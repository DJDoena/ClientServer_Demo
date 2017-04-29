using System;

namespace DoenaSoft.DVDProfiler.SampleClientServer
{
    [Serializable]
    public class TransactionObject
    {
        private static Int32 s_Counter = 0;

        public Int32 Id { get; private set; }

        public TransactionObject()
        {
            Id = ++s_Counter;
        }

        public override Boolean Equals(Object obj)
        {
            TransactionObject other = obj as TransactionObject;

            if (ReferenceEquals(other, null))
            {
                return (false);
            }

            return (Id == other.Id);
        }

        public override Int32 GetHashCode()
            => (Id.GetHashCode());

        public static Boolean operator ==(TransactionObject left
            , TransactionObject right)
        {
            if (ReferenceEquals(left, null) == false)
            {
                return (left.Equals(right));
            }

            if (ReferenceEquals(right, null) == false)
            {
                return (false);
            }

            return (true);
        }

        public static Boolean operator !=(TransactionObject left
            , TransactionObject right)
        {
            if (ReferenceEquals(left, null) == false)
            {
                return (left.Equals(right) == false);
            }

            if (ReferenceEquals(right, null) == false)
            {
                return (true);
            }

            return (false);
        }
    }
}
using System;

namespace DoenaSoft.DVDProfiler.SampleClientServer
{
    [Serializable]
    public class AccessResult<T>
    {
        public Boolean Success { get; private set; }

        public String ErrorMessage { get; protected set; }

        public T Result { get; protected set; }

        public AccessResult(T result)
        {
            Success = true;

            ErrorMessage = null;

            Result = result;
        }

        public AccessResult(String errorMessage)
        {
            Success = false;

            ErrorMessage = errorMessage;
        }

        protected AccessResult(Boolean success)
        {
            Success = success;
        }
    }
}
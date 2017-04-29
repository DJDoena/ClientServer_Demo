using System;

namespace DoenaSoft.DVDProfiler.SampleClientServer
{
    [Serializable]
    public class AccessResultOfString : AccessResult<String>
    {
        public AccessResultOfString(String text
            , Boolean isErrorMessage)
            : base(isErrorMessage == false)
        {
            if (isErrorMessage)
            {
                ErrorMessage = text;
            }
            else
            {
                Result = text;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using Hik.Communication.ScsServices.Service;

namespace DoenaSoft.DVDProfiler.SampleClientServer
{
    [ScsService]
    public interface IDVDProfilerRemoteAccess
    {
        TransactionObject BeginTransaction();

        AccessResult<Boolean> CommitTransaction(TransactionObject transactionObject);

        AccessResult<String> GetTitle(TransactionObject transactionObject, String profileId, Boolean commitTransactionImplicitly);

        AccessResult<Boolean> SetTitle(TransactionObject transactionObject, String profileId, String title, Boolean commitTransactionImplicitly);

        AccessResult<String> GetOriginalTitle(TransactionObject transactionObject, String profileId, Boolean commitTransactionImplicitly);

        AccessResult<Boolean> SetOriginalTitle(TransactionObject transactionObject, String profileId, String title, Boolean commitTransactionImplicitly);

        AccessResult<List<String>> GetAllProfileIds(TransactionObject transactionObject, Boolean commitTransactionImplicitly);

        AccessResult<Boolean> AbortTransaction(TransactionObject transactionObject);

        /// <summary>
        /// Any kind of writing operation on the server activates a timeout in which the transaction must be
        /// committed or a follow-up action (read or write) must be started.
        /// 
        /// If the timeout runs out, the transaction is aborted on the server side.
        /// 
        /// This prevents a deadlock on the server in case the client application crashes.
        /// 
        /// The default timeout is 2000 (2 seconds). 
        /// 
        /// But maybe the evaluating operation on the client is going to take longer. Then you can increase the timeout here.
        /// </summary>
        /// <param name="transactionObject">the TransactionObject</param>
        /// <param name="timeout">Timeout in millisecond</param>
        /// <returns>if operation was successful</returns>
        AccessResult<Boolean> SetTimeout(TransactionObject transactionObject, Int32 timeout, Boolean commitTransactionImplicitly);

        /// <summary>
        /// Resets the timeout to its default value.
        /// </summary>
        /// <param name="transactionObject">the TransactionObject</param>
        /// <returns>if operation was successful</returns>
        AccessResult<Boolean> ResetTimeout(TransactionObject transactionObject, Boolean commitTransactionImplicitly);

        AccessResult<ChangeNotificationObject> RegisterForProfileChanges(TransactionObject transactionObject, Boolean commitTransactionImplicitly);

        AccessResult<Boolean> UnregisterForProfileChanges(TransactionObject transactionObject, ChangeNotificationObject changeNotificationObject, Boolean commitTransactionImplicitly);

        AccessResult<List<String>> PollChangedProfiles(TransactionObject transactionObject, ChangeNotificationObject changeNotificationObject, Boolean commitTransactionImplicitly);
    }
}
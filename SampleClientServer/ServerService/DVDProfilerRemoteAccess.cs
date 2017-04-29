using System;
using System.Collections.Generic;
using System.Threading;
using Hik.Communication.ScsServices.Service;
using Invelos.DVDProfilerPlugin;

namespace DoenaSoft.DVDProfiler.SampleClientServer
{
    public class DVDProfilerRemoteAccess : ScsService, IDVDProfilerRemoteAccess
    {
        //Theoretically n clients can call this class simultaneously but
        //I doubt DVD Profiler can handle that. So all calls will made thread-safe.
        //Each client gets a transaction handle and only with that he's allowed to 
        //enter a method
        //All read-transaction will have an implicit CommitTransaction

        //We need two different thread locks, one to register for a transaction
        //one to actually do the stuff we want to do

        //For example. The first thread starts BeginTransaction and gets its ID
        //Then the next thread comes, uses the lock and waits for its ID
        //But as long as it holds the lock, the first thread won't be able to commit
        //the transaction -> deadlock

        //In case of operations without implicit commit, we should start a timer, 
        //in case the client crashes and our the transaction would remain active.
        //2 seconds and we kill the transaction

        private const Int32 TimerTimeout = 2000;

        private const String WrongTransaction = "Wrong Transaction";

        private Object m_BeginTransactionLock;

        private Object m_TransactionLock;

        private TransactionObject m_CurrentTransaction;

        private IDVDProfilerAPI m_Api;

        private System.Timers.Timer m_Timer;

        private Boolean m_TimerIsRunning;

        private Dictionary<String, IDVDInfo> m_OpenProfiles;

        private Dictionary<String, IDVDInfo> m_EditedProfiles;

        private Dictionary<ChangeNotificationObject, List<String>> m_Notifications;

        public DVDProfilerRemoteAccess(IDVDProfilerAPI api)
        {
            m_Api = api;

            m_BeginTransactionLock = new Object();

            m_TransactionLock = new Object();

            m_Timer = new System.Timers.Timer();

            m_Timer.Interval = TimerTimeout;
            m_Timer.Elapsed += OnTimerTick;

            m_EditedProfiles = new Dictionary<String, IDVDInfo>();

            m_OpenProfiles = new Dictionary<String, IDVDInfo>();

            m_Notifications = new Dictionary<ChangeNotificationObject, List<String>>();
        }

        #region IDVDProfilerRemoteAccess Members

        public TransactionObject BeginTransaction()
        {
            while (m_CurrentTransaction != null)
            {
                Thread.Sleep(250);
            }

            lock (m_BeginTransactionLock)
            {
                while (m_CurrentTransaction != null)
                {
                    Thread.Sleep(250);
                }

                m_CurrentTransaction = new TransactionObject();

                return (m_CurrentTransaction);
            }
        }

        public AccessResult<Boolean> CommitTransaction(TransactionObject transactionObject)
        {
            if (m_CurrentTransaction != transactionObject)
            {
                return (new AccessResult<Boolean>(WrongTransaction));
            }

            lock (m_TransactionLock)
            {
                PauseTimer();

                foreach (KeyValuePair<String, IDVDInfo> profileKeyValues in m_EditedProfiles)
                {
                    m_Api.SaveDVDToCollection(profileKeyValues.Value);

                    foreach (KeyValuePair<ChangeNotificationObject, List<String>> notificationKeyValues in m_Notifications)
                    {
                        if (notificationKeyValues.Value.Contains(profileKeyValues.Key) == false)
                        {
                            notificationKeyValues.Value.Add(profileKeyValues.Key);
                        }
                    }
                }

                if (m_EditedProfiles.Count > 0)
                {
                    m_Api.RequeryDatabase();
                }

                return (new AccessResult<Boolean>(EndTransaction()));
            }
        }

        public AccessResult<String> GetTitle(TransactionObject transactionObject
            , String profileId
            , Boolean commitTransactionImplicitly)
        {
            if (m_CurrentTransaction != transactionObject)
            {
                return (new AccessResultOfString(WrongTransaction, true));
            }

            lock (m_TransactionLock)
            {
                try
                {
                    PauseTimer();

                    IDVDInfo dvdInfo = GetProfileData(profileId);

                    String title = dvdInfo.GetTitle();

                    CommitOrStartTimer(transactionObject, commitTransactionImplicitly);

                    return (new AccessResultOfString(title, false));
                }
                catch (Exception ex)
                {
                    AbortTransaction(transactionObject);

                    return (new AccessResultOfString(ex.Message, true));
                }
            }
        }

        public AccessResult<Boolean> SetTitle(TransactionObject transactionObject
            , String profileId
            , String title
            , Boolean commitTransactionImplicitly)
        {
            if (m_CurrentTransaction != transactionObject)
            {
                return (new AccessResult<Boolean>(WrongTransaction));
            }

            lock (m_TransactionLock)
            {
                try
                {
                    PauseTimer();

                    IDVDInfo dvdInfo = GetProfileData(profileId);

                    dvdInfo.SetTitle(title);

                    AddProfileToEdited(profileId, dvdInfo);

                    CommitOrStartTimer(transactionObject, commitTransactionImplicitly);

                    return (new AccessResult<Boolean>(true));
                }
                catch (Exception ex)
                {
                    AbortTransaction(transactionObject);

                    return (new AccessResult<Boolean>(ex.Message));
                }
            }
        }

        public AccessResult<String> GetOriginalTitle(TransactionObject transactionObject
            , String profileId
            , Boolean commitTransactionImplicitly)
        {
            if (m_CurrentTransaction != transactionObject)
            {
                return (new AccessResultOfString(WrongTransaction, true));
            }

            lock (m_TransactionLock)
            {
                try
                {
                    PauseTimer();

                    IDVDInfo dvdInfo = GetProfileData(profileId);

                    String title = dvdInfo.GetOriginalTitle();

                    CommitOrStartTimer(transactionObject, commitTransactionImplicitly);

                    return (new AccessResultOfString(title, false));
                }
                catch (Exception ex)
                {
                    AbortTransaction(transactionObject);

                    return (new AccessResultOfString(ex.Message, true));
                }
            }
        }

        public AccessResult<Boolean> SetOriginalTitle(TransactionObject transactionObject
            , String profileId
            , String title
            , Boolean commitTransactionImplicitly)
        {
            if (m_CurrentTransaction != transactionObject)
            {
                return (new AccessResult<Boolean>(WrongTransaction));
            }

            lock (m_TransactionLock)
            {
                try
                {
                    PauseTimer();

                    IDVDInfo dvdInfo = GetProfileData(profileId);

                    dvdInfo.SetOriginalTitle(title);

                    AddProfileToEdited(profileId, dvdInfo);

                    CommitOrStartTimer(transactionObject, commitTransactionImplicitly);

                    return (new AccessResult<Boolean>(true));
                }
                catch (Exception ex)
                {
                    AbortTransaction(transactionObject);

                    return (new AccessResult<Boolean>(ex.Message));
                }
            }
        }

        public AccessResult<List<String>> GetAllProfileIds(TransactionObject transactionObject
            , Boolean commitTransactionImplicitly)
        {
            if (m_CurrentTransaction != transactionObject)
            {
                return (new AccessResult<List<String>>(WrongTransaction));
            }

            lock (m_TransactionLock)
            {
                try
                {
                    PauseTimer();

                    Object[] allIds = (Object[])(m_Api.GetAllProfileIDs());

                    List<String> profileIds = new List<String>(allIds.Length);

                    foreach (Object id in allIds)
                    {
                        profileIds.Add((String)id);
                    }

                    CommitOrStartTimer(transactionObject, commitTransactionImplicitly);

                    return (new AccessResult<List<String>>(profileIds));
                }
                catch (Exception ex)
                {
                    AbortTransaction(transactionObject);

                    return (new AccessResult<List<String>>(ex.Message));
                }
            }
        }

        public AccessResult<Boolean> AbortTransaction(TransactionObject transactionObject)
        {
            if (m_CurrentTransaction != transactionObject)
            {
                return (new AccessResult<Boolean>(WrongTransaction));
            }

            return (new AccessResult<Boolean>(EndTransaction()));
        }

        public AccessResult<Boolean> SetTimeout(TransactionObject transactionObject
            , Int32 timeout
            , Boolean commitTransactionImplicitly)
        {
            if (m_CurrentTransaction != transactionObject)
            {
                return (new AccessResult<Boolean>(WrongTransaction));
            }

            lock (m_TransactionLock)
            {
                PauseTimer();

                m_Timer.Interval = timeout;

                CommitOrStartTimer(transactionObject, commitTransactionImplicitly);

                return (new AccessResult<Boolean>(true));
            }
        }

        public AccessResult<Boolean> ResetTimeout(TransactionObject transactionObject
            , Boolean commitTransactionImplicitly)
        {
            if (m_CurrentTransaction != transactionObject)
            {
                return (new AccessResult<Boolean>(WrongTransaction));
            }

            lock (m_TransactionLock)
            {
                PauseTimer();

                m_Timer.Interval = TimerTimeout;

                CommitOrStartTimer(transactionObject, commitTransactionImplicitly);

                return (new AccessResult<Boolean>(true));
            }
        }

        public AccessResult<ChangeNotificationObject> RegisterForProfileChanges(TransactionObject transactionObject
            , Boolean commitTransactionImplicitly)
        {
            if (m_CurrentTransaction != transactionObject)
            {
                return (new AccessResult<ChangeNotificationObject>(WrongTransaction));
            }

            lock (m_TransactionLock)
            {
                ChangeNotificationObject changeNotificationObject = new ChangeNotificationObject();

                m_Notifications.Add(changeNotificationObject, new List<String>());

                CommitOrStartTimer(transactionObject, commitTransactionImplicitly);

                return (new AccessResult<ChangeNotificationObject>(changeNotificationObject));
            }
        }

        public AccessResult<Boolean> UnregisterForProfileChanges(TransactionObject transactionObject
            , ChangeNotificationObject changeNotificationObject
            , Boolean commitTransactionImplicitly)
        {
            if (m_CurrentTransaction != transactionObject)
            {
                return (new AccessResult<Boolean>(WrongTransaction));
            }

            lock (m_TransactionLock)
            {
                PauseTimer();

                if (changeNotificationObject == null)
                {
                    CommitOrStartTimer(transactionObject, commitTransactionImplicitly);

                    return (new AccessResult<Boolean>("changeNotificationObject is null"));
                }

                m_Notifications.Remove(changeNotificationObject);

                CommitOrStartTimer(transactionObject, commitTransactionImplicitly);

                return (new AccessResult<Boolean>(true));
            }
        }

        public AccessResult<List<String>> PollChangedProfiles(TransactionObject transactionObject
            , ChangeNotificationObject changeNotificationObject
            , Boolean commitTransactionImplicitly)
        {
            if (m_CurrentTransaction != transactionObject)
            {
                return (new AccessResult<List<String>>(WrongTransaction));
            }

            lock (m_TransactionLock)
            {
                PauseTimer();

                if (changeNotificationObject == null)
                {
                    CommitOrStartTimer(transactionObject, commitTransactionImplicitly);

                    return (new AccessResult<List<String>>("changeNotificationObject is null"));
                }

                List<String> changedProfiles;
                if (m_Notifications.TryGetValue(changeNotificationObject, out changedProfiles) == false)
                {
                    CommitOrStartTimer(transactionObject, commitTransactionImplicitly);

                    return (new AccessResult<List<String>>("changeNotificationObject is not registered"));
                }

                //We need to clone the list here, so we can then clear it
                List<String> returnValue = new List<String>(changedProfiles);

                changedProfiles.Clear();

                CommitOrStartTimer(transactionObject, commitTransactionImplicitly);

                return (new AccessResult<List<String>>(returnValue));
            }
        }

        #endregion

        private void OnTimerTick(Object sender
            , System.Timers.ElapsedEventArgs e)
        {
            EndTransaction();
        }

        private Boolean EndTransaction()
        {
            lock (m_TransactionLock)
            {
                m_Timer.Stop();

                m_TimerIsRunning = false;

                m_OpenProfiles.Clear();

                m_EditedProfiles.Clear();

                m_CurrentTransaction = null;

                return (true);
            }
        }

        private IDVDInfo GetProfileData(String profileId)
        {
            IDVDInfo dvdInfo;
            if (m_OpenProfiles.TryGetValue(profileId, out dvdInfo) == false)
            {
                m_Api.DVDByProfileID(out dvdInfo, profileId, -1, -1);
                m_Api.DVDByProfileID(out dvdInfo, dvdInfo.GetProfileID(), -1, -1);

                m_OpenProfiles.Add(profileId, dvdInfo);
            }

            return (dvdInfo);
        }

        private void PauseTimer()
        {
            if (m_TimerIsRunning)
            {
                m_Timer.Stop();
            }
        }

        private void StartTimer()
        {
            m_TimerIsRunning = true;

            m_Timer.Start();
        }

        private void CommitOrStartTimer(TransactionObject transactionObject
            , Boolean commitTransaction)
        {
            if (commitTransaction)
            {
                CommitTransaction(transactionObject);
            }
            else
            {
                StartTimer();
            }
        }


        private void AddProfileToEdited(String profileId
            , IDVDInfo dvdInfo)
        {
            if (m_EditedProfiles.ContainsKey(profileId) == false)
            {
                m_EditedProfiles.Add(profileId, dvdInfo);
            }
        }
    }
}
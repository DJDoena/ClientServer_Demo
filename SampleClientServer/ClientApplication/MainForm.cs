using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DoenaSoft.DVDProfiler.SampleClientServer;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.ScsServices.Client;

namespace ClientApplication
{
    public partial class MainForm : Form
    {
        private ChangeNotificationObject m_ChangeNotificationObject;

        private IScsServiceClient<IDVDProfilerRemoteAccess> m_RemoteAccessClient;

        public MainForm()
        {
            InitializeComponent();
        }

        private void OnGetAllIdsButtonClick(Object sender
            , EventArgs e)
        {
            if (m_RemoteAccessClient == null)
            {
                m_RemoteAccessClient = ScsServiceClientBuilder.CreateClient<IDVDProfilerRemoteAccess>(new ScsTcpEndPoint(IPAddressTextBox.Text, 10083));

                m_RemoteAccessClient.Connect();
            }

            ProfileIdListView.Items.Clear();

            TransactionObject transactionObject = m_RemoteAccessClient.ServiceProxy.BeginTransaction();

            if (m_ChangeNotificationObject == null)
            {
                AccessResult<ChangeNotificationObject> result = m_RemoteAccessClient.ServiceProxy.RegisterForProfileChanges(transactionObject, false);

                m_ChangeNotificationObject = result.Result;

                Timer.Interval = 5000;
                Timer.Tick += new EventHandler(OnTimer1Tick);
            }

            Timer.Stop();

            try
            {
                AccessResult<List<String>> result = m_RemoteAccessClient.ServiceProxy.GetAllProfileIds(transactionObject, false);

                if (result.Success)
                {
                    if (result.Result != null)
                    {
                        foreach (String profileId in result.Result)
                        {
                            ListViewItem item = new ListViewItem();

                            GetTitle(transactionObject, profileId, item);

                            ProfileIdListView.Items.Add(item);
                        }
                    }

                    m_RemoteAccessClient.ServiceProxy.CommitTransaction(transactionObject);
                }
                else
                {
                    m_RemoteAccessClient.ServiceProxy.AbortTransaction(transactionObject);

                    MessageBox.Show("An error occured", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                Timer.Start();
            }
            catch (Exception ex)
            {
                try
                {
                    m_RemoteAccessClient.ServiceProxy.AbortTransaction(transactionObject);
                }
                catch { }

                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private String GetTitle(TransactionObject transactionObject, String profileId, ListViewItem item)
        {
            AccessResult<String> titleResult = m_RemoteAccessClient.ServiceProxy.GetTitle(transactionObject, profileId, false);

            AccessResult<String> originalTitleResult = m_RemoteAccessClient.ServiceProxy.GetOriginalTitle(transactionObject, profileId, false);

            item.Text = titleResult.Result;

            if (String.IsNullOrEmpty(originalTitleResult.Result) == false)
            {
                item.Text += " (" + originalTitleResult.Result + ")";
            }

            item.Tag = profileId;

            return (titleResult.Result);
        }

        private void OnTimer1Tick(Object sender
            , EventArgs e)
        {
            Timer.Stop();

            if (m_ChangeNotificationObject != null)
            {
                TransactionObject transactionObject = m_RemoteAccessClient.ServiceProxy.BeginTransaction();

                AccessResult<List<String>> changedProfiles = m_RemoteAccessClient.ServiceProxy.PollChangedProfiles(transactionObject, m_ChangeNotificationObject, false);

                if (changedProfiles.Result.Count > 0)
                {
                    for (Int32 i = 0; i < ProfileIdListView.Items.Count; i++)
                    {
                        ListViewItem item = ProfileIdListView.Items[i];

                        String profileId = item.Tag.ToString();

                        if (changedProfiles.Result.Contains(profileId))
                        {
                            String title = GetTitle(transactionObject, profileId, item);

                            if ((ProfileIdListView.SelectedIndices.Count == 1)
                                && (ProfileIdListView.SelectedIndices[0] == i))
                            {
                                TitleTextBox.Text = title;
                            }
                        }
                    }
                }

                m_RemoteAccessClient.ServiceProxy.CommitTransaction(transactionObject);
            }

            Timer.Start();
        }

        private void OnProfileIdListViewSelectedIndexChanged(Object sender
            , EventArgs e)
        {
            if (ProfileIdListView.SelectedIndices.Count == 1)
            {
                String profileId = ProfileIdListView.Items[ProfileIdListView.SelectedIndices[0]].Tag.ToString();

                TransactionObject transactionObject = m_RemoteAccessClient.ServiceProxy.BeginTransaction();

                try
                {
                    AccessResult<String> result = m_RemoteAccessClient.ServiceProxy.GetTitle(transactionObject, profileId, true);

                    if (result.Success)
                    {
                        TitleTextBox.Text = result.Result;
                    }
                    else
                    {
                        MessageBox.Show("An error occured", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        m_RemoteAccessClient.ServiceProxy.AbortTransaction(transactionObject);
                    }
                    catch { }

                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void OnSetTitleButtonClick(Object sender
            , EventArgs e)
        {
            if (ProfileIdListView.SelectedIndices.Count == 1)
            {
                String profileId = ProfileIdListView.Items[ProfileIdListView.SelectedIndices[0]].Tag.ToString();

                TransactionObject transactionObject = m_RemoteAccessClient.ServiceProxy.BeginTransaction();

                try
                {
                    AccessResult<Boolean> result = m_RemoteAccessClient.ServiceProxy.SetTitle(transactionObject, profileId, TitleTextBox.Text, false);

                    m_RemoteAccessClient.ServiceProxy.CommitTransaction(transactionObject);

                    if (result.Success == false)
                    {
                        MessageBox.Show("An error occured", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        m_RemoteAccessClient.ServiceProxy.AbortTransaction(transactionObject);
                    }
                    catch { }

                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void OnMainFormClosing(Object sender
            , FormClosingEventArgs e)
        {
            Timer.Stop();

            if (m_ChangeNotificationObject != null)
            {
                TransactionObject transactionObject = m_RemoteAccessClient.ServiceProxy.BeginTransaction();

                m_RemoteAccessClient.ServiceProxy.UnregisterForProfileChanges(transactionObject, m_ChangeNotificationObject, true);

                m_ChangeNotificationObject = null;
            }

            if (m_RemoteAccessClient != null)
            {
                m_RemoteAccessClient.Disconnect();
                m_RemoteAccessClient.Dispose();
                m_RemoteAccessClient = null;
            }
        }

        private void OnSetTitleAndOriginalTitleButtonClick(Object sender
            , EventArgs e)
        {
            if (ProfileIdListView.SelectedIndices.Count == 1)
            {
                String profileId = ProfileIdListView.Items[ProfileIdListView.SelectedIndices[0]].Tag.ToString();

                TransactionObject transactionObject = m_RemoteAccessClient.ServiceProxy.BeginTransaction();

                try
                {
                    m_RemoteAccessClient.ServiceProxy.SetTimeout(transactionObject, 20000, false);//To give us time debugging;
                    m_RemoteAccessClient.ServiceProxy.SetTitle(transactionObject, profileId, "A Title", false);
                    m_RemoteAccessClient.ServiceProxy.SetOriginalTitle(transactionObject, profileId, "An Original Title", false);
                    m_RemoteAccessClient.ServiceProxy.CommitTransaction(transactionObject);
                }
                catch (Exception ex)
                {
                    try
                    {
                        m_RemoteAccessClient.ServiceProxy.AbortTransaction(transactionObject);
                    }
                    catch { }

                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
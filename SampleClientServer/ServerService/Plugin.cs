using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.ScsServices.Service;
using Invelos.DVDProfilerPlugin;

namespace DoenaSoft.DVDProfiler.SampleClientServer
{
    [ComVisible(true)]
    [Guid("6C066A06-2F0F-421F-A789-226CAA9858F6")]
    public class Plugin : IDVDProfilerPlugin, IDVDProfilerPluginInfo
    {
        private IScsServiceApplication ServiceApplication;

        public Plugin()
        { }

        #region IDVDProfilerPlugin Members
        public void Load(IDVDProfilerAPI api)
        {
            //Create a service application that runs on 10083 TCP port
            ServiceApplication = ScsServiceBuilder.CreateService(new ScsTcpEndPoint(10083));

            //Create a CalculatorService and add it to service application
            ServiceApplication.AddService<IDVDProfilerRemoteAccess, DVDProfilerRemoteAccess>(new DVDProfilerRemoteAccess(api));

            //Start service application
            ServiceApplication.Start();
        }

        public void Unload()
        {
            //Stop service application
            ServiceApplication.Stop();

            ServiceApplication = null;
        }

        public void HandleEvent(Int32 EventType
            , Object EventData)
        { }

        #endregion

        #region IDVDProfilerPluginInfo Members
        public String GetName()
            => ("SampleClient Server - ServerPlugin");

        public String GetDescription()
            => (String.Empty);

        public String GetAuthorName()
            => ("Doena Soft.");

        public String GetAuthorWebsite()
            => ("http://doena-journal.net/en/");

        public Int32 GetPluginAPIVersion()
            => (PluginConstants.API_VERSION);

        public Int32 GetVersionMajor()
        {
            Version version = Assembly.GetAssembly(GetType()).GetName().Version;

            return (version.Major);
        }

        public Int32 GetVersionMinor()
        {
            Version version = Assembly.GetAssembly(GetType()).GetName().Version;

            return (version.Minor * 100 + version.Build * 10 + version.Revision);
        }

        #endregion

        #region Plugin Registering

        [DllImport("user32.dll")]
        public extern static int SetParent(int child, int parent);

        [ComImport(), Guid("0002E005-0000-0000-C000-000000000046")]
        internal class StdComponentCategoriesMgr { }

        [ComRegisterFunction]
        public static void RegisterServer(Type t)
        {
            CategoryRegistrar.ICatRegister cr = (CategoryRegistrar.ICatRegister)new StdComponentCategoriesMgr();

            Guid clsidThis = new Guid("6C066A06-2F0F-421F-A789-226CAA9858F6");

            Guid catid = new Guid("833F4274-5632-41DB-8FC5-BF3041CEA3F1");

            cr.RegisterClassImplCategories(ref clsidThis, 1, new Guid[] { catid });
        }

        [ComUnregisterFunction]
        public static void UnregisterServer(Type t)
        {
            CategoryRegistrar.ICatRegister cr = (CategoryRegistrar.ICatRegister)new StdComponentCategoriesMgr();

            Guid clsidThis = new Guid("6C066A06-2F0F-421F-A789-226CAA9858F6");

            Guid catid = new Guid("833F4274-5632-41DB-8FC5-BF3041CEA3F1");

            cr.UnRegisterClassImplCategories(ref clsidThis, 1, new Guid[] { catid });
        }

        #endregion
    }
}
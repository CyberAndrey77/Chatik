using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Server;

namespace ServerService
{
    public partial class Service1 : ServiceBase
    {
        private DataBaseManager _dataBaseManager;
        private FileManager _fileManager;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _fileManager = new FileManager();
            _fileManager.CreateLog("Начало");
            try
            {
                var config = _fileManager.GetConfig();
                _dataBaseManager = new DataBaseManager(new Network(), config.ConnectionString)
                {
                    Network =
                    {
                        MessageHandlerDelegate = _fileManager.CreateLog
                    }
                };
                _fileManager.CreateLog("Запущенр");
                _dataBaseManager.Network.StartSever(config.WaitTimeInSecond, config.Port);
            }
            catch (Exception e)
            {
                _fileManager.CreateLog(e.Message);
            }
        }

        protected override void OnStop()
        {
            try
            {
                _dataBaseManager.Network.StopServer();
            }
            catch (Exception e)
            {
                _fileManager.CreateLog(e.Message);
            }
        }
    }
}

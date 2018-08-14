using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml.Linq;
using Talgat.Magafurov.Models;

namespace Talgat.Magafurov.ViewModels
{
    internal class CLItoXMLconverterViewModel : INotifyPropertyChanged
    {
        private bool CLIimported = false;
        private string CLIImportedString = string.Empty;
        private RelayCommand _importCLICommand;
        private RelayCommand _exportXMLCommand;
        private IEnumerable<CLItoXMLconverterModel> _items;

        #region Import CLI

        public RelayCommand ImportCLICommand
        {
            get => _importCLICommand ??
                (_importCLICommand = new RelayCommand(obj =>
                {
                    ImportCLI();
                }));
        }

        private void ImportCLI()
        {
            var stringCLI = LoadCLI();

            if (string.IsNullOrEmpty(stringCLI))
                return;

            Items = ConvertCLItoModel(stringCLI);
            CLIImportedString = stringCLI;
            CLIimported = true;
        }

        private string LoadCLI()
        {
            try
            {
                OpenFileDialog openDlg = new OpenFileDialog()
                {
                    Multiselect = false,
                    Filter = "JunOS CLI |*.cli",
                    DefaultExt = "cli"
                };
                if (openDlg.ShowDialog() == true)
                {
                    var str = "";
                    using (var sr = new StreamReader(openDlg.FileName))
                    {
                        str = sr.ReadToEnd();
                    }
                    return str;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erros", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return null;
        }

        private IEnumerable<CLItoXMLconverterModel> ConvertCLItoModel(string stringCLI)
        {
            var models = new List<CLItoXMLconverterModel>();

            var strs = Regex.Split(stringCLI, @"[\r\n]");
            foreach (var str in strs)
            {
                var strSplit = Regex.Split(str, @" +");
                if (strSplit.Count() < 5) continue;
                if (strSplit[0] != "set") continue;
                if (strSplit[1] != "applications") continue;
                string
                    applicationType = strSplit[2],
                    name = strSplit[3],
                    prop = strSplit[4],
                    value = strSplit[5];

                var curModel = models.Find(x => x.Name == name);
                var curModelIsNew = false;
                if (curModel is null)
                {
                    curModel = new CLItoXMLconverterModel()
                    {
                        Name = name,
                        ApplicationType = applicationType
                    };
                    curModelIsNew = true;
                }

                switch (applicationType)
                {
                    case "application":
                        if (SetProperty(curModel, prop, value) && curModelIsNew)
                        {
                            models.Add(curModel);
                        }
                        break;

                    case "application-set":
                        string
                            appType = prop,
                            appName = value;
                        if (AddInMembers(curModel, appType, appName) && curModelIsNew)
                        {
                            models.Add(curModel);
                        }
                        break;

                    default: break;
                }
            }

            return models;
        }

        private bool SetProperty(CLItoXMLconverterModel model, string propName, string value)
        {
            switch (propName.ToLower())
            {
                case "protocol":
                    model.Protocol = value;
                    break;

                case "source-port":
                    model.SourcePort = value;
                    break;

                case "destination-port":
                    model.DestinationPort = value;
                    break;

                case "description":
                    model.Description = value;
                    break;

                default:
                    return false;
            }
            return true;
        }

        private bool AddInMembers(CLItoXMLconverterModel model, string membersType, string membersName)
        {
            model.MembersList.Add(
                new CLItoXMLconverterModel()
                {
                    Name = membersName,
                    ApplicationType = membersType
                });
            return true;
        }

        #endregion Import CLI

        #region Export XML

        public RelayCommand ExportXMLCommand
        {
            get => _exportXMLCommand ??
                (_exportXMLCommand = new RelayCommand(obj =>
                    {
                        ExportXML();
                    },
                    obj => (CLIimported == true && Items.Any())));
        }

        private void ExportXML()
        {
            try
            {
                SaveFileDialog saveDlg = new SaveFileDialog()
                {
                    FileName = "JunOS XML",
                    Filter = "JunOS XML |*.xml",
                    DefaultExt = "xml"
                };
                if (saveDlg.ShowDialog() == true)
                {
                    XDocument xml = ConvertModelToXML(Items);
                    xml.Save(saveDlg.FileName);
                    Process.Start(saveDlg.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erros", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private XDocument ConvertModelToXML(IEnumerable<CLItoXMLconverterModel> models)
        {
            var xDoc = new XDocument(
                new XElement("applications",
                     models.Select(x => GetXElement(x))));

            return xDoc;
        }

        private XElement GetXElement(CLItoXMLconverterModel model)
        {
            XElement element =
                new XElement(model.ApplicationType,
                    new XElement("name", model.Name));

            if (!string.IsNullOrEmpty(model.Protocol))
                element.Add(new XElement("protocol", model.Protocol));

            if (!string.IsNullOrEmpty(model.SourcePort))
                element.Add(new XElement("source-port", model.SourcePort));

            if (!string.IsNullOrEmpty(model.DestinationPort))
                element.Add(new XElement("destination-port", model.DestinationPort));

            if (!string.IsNullOrEmpty(model.Description))
                element.Add(new XElement("description", model.Description));

            foreach (var member in model.MembersList)
                element.Add(
                    GetXElement(member));

            return element;
        }

        #endregion Export XML

        public IEnumerable<CLItoXMLconverterModel> Items
        {
            get => _items ?? (_items = new List<CLItoXMLconverterModel>());
            set
            {
                _items = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
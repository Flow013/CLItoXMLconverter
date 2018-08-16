using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using Talgat.Magafurov.Components.ViewModels;
using Talgat.Magafurov.Models;
using Talgat.Magafurov.ViewModels.Components;

namespace Talgat.Magafurov.ViewModels
{
    internal class CLItoXMLconverterViewModel : BaseViewModel
    {
        private bool CLIimported = false;
        private string CLIImportedString = string.Empty;
        private RelayCommand _importCLICommand;
        private RelayCommand _exportXMLCommand;
        private IEnumerable<CLItoXMLconverterModel> _items;
        private ProgressBarProp _progressBarProp = new ProgressBarProp();

        #region Import CLI

        public RelayCommand ImportCLICommand
        {
            get => _importCLICommand ??
                (_importCLICommand = new RelayCommand(obj =>
                {
                    ImportCLIAsync();
                }));
        }

        private async Task ImportCLIAsync()
        {
            var stringCLI = LoadCLI();

            if (string.IsNullOrEmpty(stringCLI))
                return;

            Items = await ConvertCLItoModel(stringCLI);
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
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return null;
        }

        private async Task<IEnumerable<CLItoXMLconverterModel>> ConvertCLItoModel(string stringCLI)
        {
            var models = new List<CLItoXMLconverterModel>();

            var strs = Regex.Split(stringCLI, @"[\r\n]");
            ProgressBarProp.ProgressBarMax = strs.Length + 1;
            foreach (var str in strs)
            {
                await ActionTask(() =>
                {
                    var model = GetCLItoXMLconverterModel(str, models);
                    if (model != null)
                        models.Add(model);

                    ProgressBarProp.ProgressBarValue++;
                    Thread.Sleep(300);
                });
            }
            ProgressBarProp.ProgressBarValue = 0;

            return models;
        }

        private CLItoXMLconverterModel GetCLItoXMLconverterModel(string inputString, IEnumerable<CLItoXMLconverterModel> models)
        {
            string[] strSplit = Regex.Split(inputString, @" +");
            if (strSplit.Count() < 5) return null;
            if (strSplit[0] != "set") return null;
            if (strSplit[1] != "applications") return null;
            string
                applicationType = strSplit[2],
                name = strSplit[3],
                prop = strSplit[4],
                value = strSplit[5];

            var curModelIsNew = false;
            var curModel = models.FirstOrDefault(x => x.Name == name);
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
                        return curModel;
                    }
                    return null;

                case "application-set":
                    string
                        appType = prop,
                        appName = value;
                    if (AddInMembers(curModel, appType, appName) && curModelIsNew)
                    {
                        return curModel;
                    }
                    return null;

                default: return null;
            }
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
                    ExportXMLAsync();
                },
                    obj => (CLIimported == true && Items.Any())));
        }

        private async Task ExportXMLAsync()
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
                    XDocument xml = await ConvertModelToXMLAsync(Items);
                    xml.Save(saveDlg.FileName);
                    Process.Start(saveDlg.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task<XDocument> ConvertModelToXMLAsync(IEnumerable<CLItoXMLconverterModel> models)
        {
            ProgressBarProp.ProgressBarValue = 0;
            ProgressBarProp.ProgressBarMax = models.Count();
            var xDoc = new XDocument();
            var applicationXElem = new XElement("applications");
            xDoc.Add(applicationXElem);
            foreach (var model in models)
            {
                await ActionTask(() =>
                {
                    Thread.Sleep(300);
                    ProgressBarProp.ProgressBarValue++;
                    applicationXElem.Add(GetXElement(model));
                });
            }

            ProgressBarProp.ProgressBarValue = 0;
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
        public ProgressBarProp ProgressBarProp
        {
            get => _progressBarProp;
            set
            {
                _progressBarProp = value;
                OnPropertyChanged(nameof(ProgressBarProp));
            }
        }
    }
}
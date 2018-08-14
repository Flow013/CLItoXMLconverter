using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Talgat.Magafurov.Models
{
    internal class CLItoXMLconverterModel
    {
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Не смог сделать конвертор, так как старался придерживаться условия:
        /// Предусмотреть возможность автоматической генерации столбцов таблицы на основе
        /// модели(используя Reflections). Что бы при изменении модели – отображение изменялось соответственно.
        /// </summary>
        [Display(Name = "Type")]
        public string Type
        {
            get
            {
                switch (ApplicationType)
                {
                    case "application"://JunOSApplicationTypes.application:
                        return "Service";

                    case "application-set"://JunOSApplicationTypes.applicationSet:
                        return "Group";

                    default:
                        return "";
                }
            }
        }

        [Display(AutoGenerateField = false)]
        public string ApplicationType { get; set; }

        [Display(Name = "Protocol")]
        public string Protocol { get; set; }

        [Display(Name = "Source Port")]
        public string SourcePort { get; set; }

        [Display(Name = "Destination Port")]
        public string DestinationPort { get; set; }

        [Display(AutoGenerateField = false)]
        public string Description { get; set; }

        [Display(Name = "Members")]
        public string Members => string.Join(", ", MembersList.Select(x => x.Name));

        [Display(AutoGenerateField = false)]
        public List<CLItoXMLconverterModel> MembersList { get; set; } = new List<CLItoXMLconverterModel>();
    }
}
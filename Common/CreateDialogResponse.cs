using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class CreateDialogResponse
    {
        public string CreatorName { get; set; }
        public string InventedName { get; set; }

        public CreateDialogResponse(string creator, string invented)
        {
            CreatorName = creator;
            InventedName = invented;
        }

        public MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Identifier = nameof(CreateDialogResponse),
                Payload = this
            };

            return container;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using WeMoLib;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace WeMoWeb
{
    [Route("/wemo/lamp")]
    public class WeMoController : Controller
    {
        [HttpPost("on")]
        public void TurnOn()
        {
            WeMoSwitch.ConnectTo("Lamp")?.TurnOn();
        }

        [HttpPost("off")]
        public void TurnOff()
        {
            WeMoSwitch.ConnectTo("Lamp")?.TurnOff();
        }

        [HttpPost("toggle")]
        public void Toggle()
        {
            var lamp = WeMoSwitch.ConnectTo("Lamp");
            if (lamp.IsOff())
            {
                lamp.TurnOn();
            }
            else
            {
                lamp.TurnOff();
            }
        }
    }
}

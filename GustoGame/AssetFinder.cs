using Gusto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto
{
    public class AssetFinder
    {
        public static Dictionary<string, Asset> Ships = new Dictionary<string, Asset>();
        public static Dictionary<string, Asset> Sails = new Dictionary<string, Asset>();
        public static Dictionary<string, Asset> Towers = new Dictionary<string, Asset>();
    }
}

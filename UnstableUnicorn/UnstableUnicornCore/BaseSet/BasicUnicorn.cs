using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnstableUnicornCore.BaseSet {
    public class BasicUnicorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return Card
                .Name("Basic Unicorn")
                .Text("Just bacis unicorn.");
        }
    }
}

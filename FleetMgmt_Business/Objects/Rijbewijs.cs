using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Objects {
    public class Rijbewijs {

        public string Categorie { get; set; }

        public DateTime BehaaldOp { get; set; }

        public Rijbewijs(string categorie, DateTime behaaldop) {
            this.Categorie = categorie;
            this.BehaaldOp = behaaldop;
        }

    }
}

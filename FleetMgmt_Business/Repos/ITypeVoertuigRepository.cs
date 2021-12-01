using FleetMgmt_Business.Enums;
using FleetMgmt_Business.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Repos {
    public interface ITypeVoertuigRepository {

        void voegTypeVoertuigToe(TypeVoertuig type);
        TypeVoertuig updateTypeVoertuig(TypeVoertuig type);
        void verwijderTypeVoertuig(TypeVoertuig type);
        TypeVoertuig verkrijgTypeVoertuig(string type, RijbewijsEnum rijbewijs);
        ICollection<TypeVoertuig> verkrijgVoertuigen(string type, RijbewijsEnum? rijbewijs);
    }
}

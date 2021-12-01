using FleetMgmt_Business.Enums;
using FleetMgmt_Business.Objects;
using FleetMgmt_Business.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Managers {
    public class TypeVoertuigManager : ITypeVoertuigRepository {

        private ITypeVoertuigRepository repo;

        public TypeVoertuigManager(ITypeVoertuigRepository repo) {
            this.repo = repo;
        }

        public TypeVoertuig updateTypeVoertuig(TypeVoertuig type) {
            return repo.updateTypeVoertuig(type);
        }

        public TypeVoertuig verkrijgTypeVoertuig(string type, RijbewijsEnum rijbewijs) {
            return repo.verkrijgTypeVoertuig(type, rijbewijs);
        }

        public ICollection<TypeVoertuig> verkrijgVoertuigen(string type, RijbewijsEnum? rijbewijs) {
            return repo.verkrijgVoertuigen(type, rijbewijs);
        }

        public void verwijderTypeVoertuig(TypeVoertuig type) {
            repo.verwijderTypeVoertuig(type);
        }

        public void voegTypeVoertuigToe(TypeVoertuig type) {
            repo.voegTypeVoertuigToe(type);
        }
    }
}

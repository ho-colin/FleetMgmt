using FleetMgmt_Business.Enums;
using FleetMgmt_Business.Exceptions;
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
            if (type.Equals(this.verkrijgTypeVoertuig(type.Type, type.vereistRijbewijs))) {
                throw new TypeVoertuigException("TypeVoertuigManager : Er is niks gewijzigd.");
            }
            return repo.updateTypeVoertuig(type);
        }

        public TypeVoertuig verkrijgTypeVoertuig(string type, RijbewijsEnum rijbewijs) {
            return repo.verkrijgTypeVoertuig(type, rijbewijs);
        }

        public ICollection<TypeVoertuig> verkrijgTypeVoertuigen(string type, RijbewijsEnum? rijbewijs) {
            return repo.verkrijgTypeVoertuigen(type, rijbewijs);
        }

        public void verwijderTypeVoertuig(TypeVoertuig type) {
            if(repo.verkrijgTypeVoertuig(type.Type,type.vereistRijbewijs) == null) { throw new TypeVoertuigException("TypeVoertuigManager : Voertuig niet gevonden!"); } 
            repo.verwijderTypeVoertuig(type);
        }

        public void voegTypeVoertuigToe(TypeVoertuig type) {
            if(repo.verkrijgTypeVoertuig(type.Type, type.vereistRijbewijs) != null) { throw new TypeVoertuigException("TypeVoertuigManager: Voertuig bestaat al!"); } 
            repo.voegTypeVoertuigToe(type);
        }
    }
}

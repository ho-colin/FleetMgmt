﻿using FleetMgmt_Business.Enums;
using FleetMgmt_Business.Exceptions;
using FleetMgmt_Business.Objects;
using FleetMgmt_Business.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Managers {
    //COLIN MEERSCHMAN
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
            if(repo.verkrijgTypeVoertuig(type.Type,type.vereistRijbewijs) == null) { throw new TypeVoertuigException("TypeVoertuigManager : TypeVoertuig niet gevonden!"); }
            if(repo.wordtTypeGebruikt(type.Type)) { throw new TypeVoertuigException("TypeVoertuigManager : Type is nog in gebruik!"); }
            repo.verwijderTypeVoertuig(type);
        }

        public void voegTypeVoertuigToe(TypeVoertuig type) {
            if(repo.verkrijgTypeVoertuig(type.Type, type.vereistRijbewijs) != null) { throw new TypeVoertuigException("TypeVoertuigManager: TypeVoertuig bestaat al!"); } 
            repo.voegTypeVoertuigToe(type);
        }

        public bool wordtTypeGebruikt(string type) {
            return repo.wordtTypeGebruikt(type);
        }
    }
}

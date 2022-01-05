USE FleetMgmt;

CREATE TABLE Voertuig(
	Chassisnummer nvarchar(17) NOT NULL PRIMARY KEY,
	Merk nvarchar(50) NOT NULL,
	Model nvarchar(50) NOT NULL,
	Nummerplaat nvarchar(9) NOT NULL,
	Brandstof nvarchar(20) NOT NULL,
	TypeVoertuig nvarchar(20) NOT NULL,
	Kleur nvarchar(20) NULL,
	AantalDeuren integer NULL,
	Bestuurder nvarchar(15) NULL

);

CREATE TABLE TypeVoertuig(
	TypeVoertuig nvarchar(20) NOT NULL PRIMARY KEY,
	Rijbewijs nvarchar(5) NOT NULL,
);


CREATE TABLE Bestuurder(
	Rijksregisternummer nvarchar(15) PRIMARY KEY NOT NULL,
	Naam nvarchar(30) NOT NULL,
	Achternaam nvarchar(30) NOT NULL,
	Geboortedatum date NOT NULL,
	TankkaartId integer NULL,
	VoertuigChassisnummer nvarchar(17) NULL
);

CREATE TABLE BestuurderRijbewijs(
	Bestuurder nvarchar(15) NOT NULL,
	Categorie nvarchar(5) NOT NULL,
	Behaald date NOT NULL
	PRIMARY KEY(Bestuurder,Categorie)
);


CREATE TABLE Tankkaart(
	Id integer NOT NULL IDENTITY(1,1) PRIMARY KEY,
	GeldigDatum date NOT NULL,
	Pincode nvarchar(4) NULL,
	Bestuurder nvarchar(15) NULL,
	Geblokkeerd bit NOT NULL
);

CREATE TABLE TankkaartBrandstof(
	TankkaartId integer NOT NULL,
	Brandstof nvarchar(20) NOT NULL,

	PRIMARY KEY (TankkaartId,Brandstof)
);


ALTER TABLE Voertuig ADD CONSTRAINT FK_Voertuig_TypeVoertuig FOREIGN KEY (TypeVoertuig) REFERENCES TypeVoertuig(TypeVoertuig);
ALTER TABLE Voertuig ADD CONSTRAINT FK_Voertuig_Bestuurder FOREIGN KEY (Bestuurder) REFERENCES Bestuurder(Rijksregisternummer);

ALTER TABLE BestuurderRijbewijs ADD CONSTRAINT FK_BestuurderRijbewijs_Bestuurder FOREIGN KEY (Bestuurder) REFERENCES Bestuurder(Rijksregisternummer);

ALTER TABLE TankkaartBrandstof ADD CONSTRAINT FK_TankkaartBrandstof_TankkaartId FOREIGN KEY (TankkaartId) REFERENCES Tankkaart(Id);

ALTER TABLE Bestuurder ADD CONSTRAINT FK_Bestuurder_TankkaartId FOREIGN KEY (TankkaartId) REFERENCES Tankkaart(Id);
ALTER TABLE Bestuurder ADD CONSTRAINT FK_Bestuurder_Chassisnummer FOREIGN KEY (VoertuigChassisnummer) REFERENCES Voertuig(Chassisnummer);

ALTER TABLE Tankkaart ADD CONSTRAINT FK_Tankkaart_Bestuurder FOREIGN KEY (Bestuurder) REFERENCES Bestuurder(Rijksregisternummer);

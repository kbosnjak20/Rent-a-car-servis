
CREATE TABLE Zaposlenik
(
    ID_Zaposlenik   INT IDENTITY(1,1) PRIMARY KEY,
    Ime             NVARCHAR(50)  NOT NULL,
    Prezime         NVARCHAR(50)  NOT NULL,
    KorisnickoIme   NVARCHAR(50)  NOT NULL UNIQUE,
    Lozinka         NVARCHAR(100) NOT NULL,
    Uloga           NVARCHAR(20)  NOT NULL
        CHECK (Uloga IN (N'Zaposlenik', N'VoditeljServisa'))
);

CREATE TABLE Klijent
(
    ID_Klijent  INT IDENTITY(1,1) PRIMARY KEY,
    Ime         NVARCHAR(50)  NOT NULL,
    Prezime     NVARCHAR(50)  NOT NULL,
    Kontakt     NVARCHAR(100) NULL
);

CREATE TABLE Vozilo
(
    ID_Vozilo           INT IDENTITY(1,1) PRIMARY KEY,
    Naziv               NVARCHAR(50)  NOT NULL,
    Marka               NVARCHAR(50)  NOT NULL,
    Godiste             INT           NOT NULL,
    BrojRegistracije    NVARCHAR(20)  NOT NULL UNIQUE,
    DatumRegistracije   DATE          NOT NULL,
    TipGoriva           NVARCHAR(10)  NOT NULL
        CHECK (TipGoriva IN (N'Dizel', N'Benzin')),
    ProsjecnaPotrosnja  FLOAT         NOT NULL,
    Kategorija          NVARCHAR(20)  NOT NULL
        CHECK (Kategorija IN (N'OsobnoVozilo', N'PutnickiKombi', N'TeretniKombi', N'Limuzina')),
    CijenaPoSatu        DECIMAL(10,2) NOT NULL,
    CijenaPoDanu        DECIMAL(10,2) NOT NULL,
    PocetnoStanjeKm     INT           NOT NULL DEFAULT 0,
    TrenutnoStanjeKm    INT           NOT NULL DEFAULT 0
);

CREATE TABLE Servis
(
    ID_Servis   INT IDENTITY(1,1) PRIMARY KEY,
    ID_Vozilo   INT NOT NULL,
    DatumOd     DATETIME NOT NULL,
    DatumDo     DATETIME NOT NULL,
    Cijena      DECIMAL(10,2) NULL,
    CONSTRAINT FK_Servis_Vozilo FOREIGN KEY (ID_Vozilo) REFERENCES Vozilo (ID_Vozilo)
);

CREATE TABLE Rezervacija
(
    ID_Rezervacija          INT IDENTITY(1,1) PRIMARY KEY,
    ID_Vozilo               INT NOT NULL,
    ID_Klijent              INT NOT NULL,
    ID_Zaposlenik           INT NOT NULL,
    DatumPocetka            DATETIME NOT NULL,
    DatumZavrsetka          DATETIME NOT NULL,
    TipNajma                NVARCHAR(10) NOT NULL
        CHECK (TipNajma IN (N'Sat', N'Dan')),
    Status                  NVARCHAR(20) NOT NULL DEFAULT N'Aktivna'
        CHECK (Status IN (N'Aktivna', N'Zavrsena', N'Otkazana')),
    StanjeKmPreuzimanje     INT NULL,
    StanjeKmPovrat          INT NULL,
    OpisOstecenja           NVARCHAR(500) NULL,
    IznosNajma              DECIMAL(10,2) NULL,
    CONSTRAINT FK_Rezervacija_Vozilo     FOREIGN KEY (ID_Vozilo)     REFERENCES Vozilo (ID_Vozilo),
    CONSTRAINT FK_Rezervacija_Klijent    FOREIGN KEY (ID_Klijent)    REFERENCES Klijent (ID_Klijent),
    CONSTRAINT FK_Rezervacija_Zaposlenik FOREIGN KEY (ID_Zaposlenik) REFERENCES Zaposlenik (ID_Zaposlenik)
);

INSERT INTO Zaposlenik (Ime, Prezime, KorisnickoIme, Lozinka, Uloga)
VALUES
    (N'Ivan', N'Horvat', N'ihorvat', N'ihorvat123', N'Zaposlenik'),
    (N'Ana',  N'Kovač',  N'akovac',  N'akovac123',  N'VoditeljServisa');

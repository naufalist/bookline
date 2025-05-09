CREATE TABLE "Customers" (
    "Id" UUID PRIMARY KEY,
    "FullName" VARCHAR(255) NOT NULL,
    "Email" VARCHAR(255) NOT NULL,
    "PasswordHash" VARCHAR(255) NOT NULL,
    "CreatedBy" VARCHAR(100),
    "CreatedAt" TIMESTAMP,
    "UpdatedBy" VARCHAR(100),
    "UpdatedAt" TIMESTAMP,
    "DeletedBy" VARCHAR(100),
    "DeletedAt" TIMESTAMP
);

CREATE TABLE "Appointments" (
    "Id" UUID PRIMARY KEY,
    "CustomerId" UUID NOT NULL,
    "Date" DATE NOT NULL,
    "Token" VARCHAR(10) NOT NULL,
    "CreatedBy" VARCHAR(100),
    "CreatedAt" TIMESTAMP,
    "UpdatedBy" VARCHAR(100),
    "UpdatedAt" TIMESTAMP,
    "DeletedBy" VARCHAR(100),
    "DeletedAt" TIMESTAMP,

    CONSTRAINT fk_appointment_customer
        FOREIGN KEY ("CustomerId") REFERENCES "Customers"("Id") ON DELETE CASCADE
);

CREATE TABLE "AppointmentDayConfigs" (
    "Id" UUID PRIMARY KEY,
    "Date" DATE UNIQUE NOT NULL,
    "IsHoliday" BOOLEAN NOT NULL,
    "MaxAppointments" INT NOT NULL,
    "CreatedBy" VARCHAR(100),
    "CreatedAt" TIMESTAMP,
    "UpdatedBy" VARCHAR(100),
    "UpdatedAt" TIMESTAMP,
    "DeletedBy" VARCHAR(100),
    "DeletedAt" TIMESTAMP
);
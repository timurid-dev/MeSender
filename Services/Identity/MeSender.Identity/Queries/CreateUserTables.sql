CREATE TABLE IF NOT EXISTS "Users"
(
    Id        UUID PRIMARY KEY,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL
);

CREATE TABLE IF NOT EXISTS "UserAuth"
(
    Id                      UUID         PRIMARY KEY,
    UserId                  UUID         NOT NULL,
    Email                   VARCHAR(255) NOT NULL,
    Password                TEXT         NOT NULL,
    Salt                    TEXT         NOT NULL,
    RefreshToken            TEXT         NULL,
    RefreshTokenExpiresAt   TIMESTAMP    WITH TIME ZONE NULL,
    CONSTRAINT FK_UserAuthentications_Users FOREIGN KEY (UserId) REFERENCES "Users" (Id),
    CONSTRAINT UQ_UserAuthentications_Email UNIQUE (Email)
);

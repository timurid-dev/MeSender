CREATE TABLE IF NOT EXISTS "UserRefreshTokens"
(
    Id           UUID PRIMARY KEY,
    UserId       UUID NOT NULL,
    Provider     VARCHAR(64) NULL,
    RefreshToken TEXT NOT NULL,
    ExpiresAt    TIMESTAMP WITH TIME ZONE NOT NULL,
    CONSTRAINT FK_UserRefreshTokens_Users FOREIGN KEY (UserId) REFERENCES "Users" (Id),
    CONSTRAINT UQ_UserRefreshTokens_RefreshToken UNIQUE (RefreshToken)
);

CREATE INDEX IF NOT EXISTS idx_userrefreshtokens_expiresat ON "UserRefreshTokens" (ExpiresAt); 
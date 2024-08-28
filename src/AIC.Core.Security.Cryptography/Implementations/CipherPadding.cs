﻿namespace AIC.Core.Security.Cryptography.Implementations;

public enum CipherPadding
{
    NOPADDING,
    RAW,
    ISO10126PADDING,
    ISO10126D2PADDING,
    ISO10126_2PADDING,
    ISO7816_4PADDING,
    ISO9797_1PADDING,
    ISO9796_1,
    ISO9796_1PADDING,
    OAEP,
    OAEPPADDING,
    OAEPWITHMD5ANDMGF1PADDING,
    OAEPWITHSHA1ANDMGF1PADDING,
    OAEPWITHSHA_1ANDMGF1PADDING,
    OAEPWITHSHA224ANDMGF1PADDING,
    OAEPWITHSHA_224ANDMGF1PADDING,
    OAEPWITHSHA256ANDMGF1PADDING,
    OAEPWITHSHA_256ANDMGF1PADDING,
    OAEPWITHSHA256ANDMGF1WITHSHA256PADDING,
    OAEPWITHSHA_256ANDMGF1WITHSHA_256PADDING,
    OAEPWITHSHA256ANDMGF1WITHSHA1PADDING,
    OAEPWITHSHA_256ANDMGF1WITHSHA_1PADDING,
    OAEPWITHSHA384ANDMGF1PADDING,
    OAEPWITHSHA_384ANDMGF1PADDING,
    OAEPWITHSHA512ANDMGF1PADDING,
    OAEPWITHSHA_512ANDMGF1PADDING,
    PKCS1,
    PKCS1PADDING,
    PKCS5,
    PKCS5PADDING,
    PKCS7,
    PKCS7PADDING,
    TBCPADDING,
    WITHCTS,
    X923PADDING,
    ZEROBYTEPADDING
}
namespace Domain.Common
{
    public class TokenSettings
    {
        /// <summary>
        /// Chave secreta para assinar os tokens (mínimo 32 caracteres)
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// Quem emite o token
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// Para quem o token é válido
        /// </summary>
        public string Audience { get; set; } = string.Empty;

        /// <summary>
        /// Expiração do token em minutos
        /// </summary>
        public int ExpiryInMinutes { get; set; } = 0;

        /// <summary>
        /// Expiração do refresh token em dias
        /// </summary>
        public int RefreshTokenExpiryInDays { get; set; } = 0;
    }
}

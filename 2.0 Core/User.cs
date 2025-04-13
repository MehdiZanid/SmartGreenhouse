using System.ComponentModel.DataAnnotations;

namespace SmartGreenhouse._2._0_Core
{
    /// <summary>
    /// Representeert een gebruiker. Ontworpen voor EF Core en Razor Pages.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Primary Key.
        /// </summary>
        public int UserId { get; private set; }

        /// <summary>
        /// Gebruikersnaam. Verplicht en uniek (uniekheid afdwingen in DbContext of DB).
        /// </summary>
        [Required(ErrorMessage = "Gebruikersnaam is verplicht.")]
        [StringLength(100)]
        public string Username { get; private set; } // Private set

        // Voeg hier later eventueel PasswordHash, Role etc. toe (ook met private set)

        /// <summary>
        /// Parameterloze constructor nodig voor EF Core en model binding.
        /// </summary>
        public User()
        {
            Username = string.Empty; // Initialiseer
        }

        /// <summary>
        /// Publieke constructor om een valide gebruiker aan te maken.
        /// </summary>
        /// <param name="username">Verplichte gebruikersnaam.</param>
        public User(string username) : this()
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException(nameof(username), "Gebruikersnaam mag niet leeg zijn.");
            Username = username;
        }

        /// <summary>
        /// Optioneel: Methode om Username te wijzigen indien nodig.
        /// </summary>
        public void UpdateUsername(string newUsername)
        {
            if (string.IsNullOrWhiteSpace(newUsername))
                throw new ArgumentException("Nieuwe gebruikersnaam mag niet leeg zijn.", nameof(newUsername));
            // Hier eventueel extra validatie of logging
            Username = newUsername;
        }
    }
}

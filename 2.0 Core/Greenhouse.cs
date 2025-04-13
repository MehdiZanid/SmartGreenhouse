using System.ComponentModel.DataAnnotations;

namespace SmartGreenhouse._2._0_Core
{
    /// <summary>
    /// Representeert een kas. Ontworpen voor EF Core en Razor Pages.
    /// </summary>
    public class Greenhouse
    {
        private readonly List<Sensor> _sensors = new List<Sensor>();

        public int GreenhouseId { get; private set; }

        /// <summary>
        /// Naam van de kas. Verplicht. Ingesteld via constructor, aanpasbaar via UpdateName.
        /// </summary>
        [Required(ErrorMessage = "Kas naam is verplicht.")] // Validatie-attribute
        public string Name { get; private set; }

        public string? Location { get; set; } // Optioneel, kan null zijn

        public IReadOnlyCollection<Sensor> Sensors => _sensors.AsReadOnly();

        /// <summary>
        /// Parameterloze constructor nodig voor EF Core en model binding.
        /// </summary>
        public Greenhouse()
        {
            // Initialiseer properties die niet nullable zijn om null-warnings te voorkomen
            Name = string.Empty;
        }

        /// <summary>
        /// Publieke constructor om een valide kas programmatisch aan te maken.
        /// </summary>
        public Greenhouse(string name, string? location = null) : this()
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name), "Kas naam mag niet leeg zijn.");
            }

            Name = name;
            Location = location;
        }

        public void UpdateName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                throw new ArgumentException("Nieuwe kas naam mag niet leeg zijn.", nameof(newName));
            }

            // Hier eventueel extra validatie of logging toevoegen
            Name = newName;
        }

        public void AddSensor(Sensor sensor)
        {
            if (sensor == null) throw new ArgumentNullException(nameof(sensor));
            // Voorkom duplicaten op basis van object referentie of (indien ID > 0) op ID
            if (!_sensors.Any(s =>
                    (s.SensorId != 0 && s.SensorId == sensor.SensorId) || object.ReferenceEquals(s, sensor)))
            {
                // Zorg dat de bi-directionele relatie klopt
                if (sensor.Greenhouse != this)
                {
                    // Verwijder van eventuele oude kas
                    sensor.Greenhouse?.RemoveSensorInternal(sensor);
                    // Zet de relatie vanuit de sensor correct (via interne methode)
                    sensor.SetGreenhouseInternal(this);
                }

                _sensors.Add(sensor);
            }
        }

        public void RemoveSensor(Sensor sensor)
        {
            if (sensor == null) throw new ArgumentNullException(nameof(sensor));
            if (_sensors.Contains(sensor))
            {
                // Verbreek de link vanuit de sensor (via interne methode)
                sensor.SetGreenhouseInternal(null);
                _sensors.Remove(sensor);
            }
        }

        /// <summary>
        /// Interne methode (binnen dezelfde assembly) om relatie te beheren.
        /// Nodig zodat Greenhouse de sensor uit _zijn_ lijst kan halen als
        /// de sensor direct aan een andere kas wordt gekoppeld.
        /// </summary>
        internal void RemoveSensorInternal(Sensor sensor)
        {
            _sensors.Remove(sensor);
        }
    }
}


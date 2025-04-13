using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SmartGreenhouse._2._0_Core
{
    /// <summary>
    /// Representeert een meting. Ontworpen voor EF Core en Razor Pages.
    /// Eigenschappen zijn onveranderlijk na creatie.
    /// </summary>
    public class Measurement
    {
        /// <summary>
        /// Primary Key.
        /// </summary>
        public int MeasurementId { get; private set; }

        /// <summary>
        /// Tijdstip. Verplicht.
        /// </summary>
        [Required]
        public DateTime Timestamp { get; private set; }

        /// <summary>
        /// Gemeten waarde. Verplicht.
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10, 2)")] // Goed voor MySQL DECIMAL type
        public decimal Value { get; private set; }

        // --- Relatie naar Sensor ---
        /// <summary>
        /// Foreign Key naar Sensor. Wordt intern beheerd.
        /// </summary>
        public int SensorId { get; private set; }

        /// <summary>
        /// Navigation property naar Sensor. Wordt intern beheerd.
        /// </summary>
        public Sensor? Sensor { get; private set; } // Private set

        /// <summary>
        /// Parameterloze constructor nodig voor EF Core en model binding.
        /// </summary>
        public Measurement()
        { }

        /// <summary>
        /// Publieke constructor om een valide meting te maken en te koppelen.
        /// </summary>
        /// <param name="timestamp">Tijdstip van meting.</param>
        /// <param name="value">Gemeten waarde.</param>
        /// <param name="sensor">De sensor die de meting deed.</param>
        public Measurement(DateTime timestamp, decimal value, Sensor sensor) : this()
        {
            if (sensor == null) throw new ArgumentNullException(nameof(sensor), "Meting moet aan een sensor gekoppeld zijn.");

            Timestamp = timestamp;
            Value = value;
            // Koppelen via de Sensor methode zorgt voor consistentie
            sensor.AddMeasurement(this);
            // SetSensorInternal is nu impliciet aangeroepen door AddMeasurement
        }

        /// <summary>
        /// Interne methode (binnen dezelfde assembly) om de Sensor relatie te zetten.
        /// Wordt aangeroepen vanuit Sensor.AddMeasurement.
        /// </summary>
        internal void SetSensorInternal(Sensor sensor)
        {
            // Hier geen check op oude sensor nodig, meting hoort altijd bij 1 sensor.
            this.Sensor = sensor;
            this.SensorId = sensor.SensorId; // SensorId moet al gezet zijn op de sensor
        }
    }
}

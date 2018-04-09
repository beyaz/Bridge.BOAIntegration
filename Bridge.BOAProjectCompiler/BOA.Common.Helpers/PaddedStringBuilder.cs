using System.Text;

namespace BOA.Common.Helpers
{
    /// <summary>
    ///     The padded string builder
    /// </summary>
    public class PaddedStringBuilder
    {
        #region Fields
        /// <summary>
        ///     The sb
        /// </summary>
        readonly StringBuilder sb = new StringBuilder();
        #endregion

        #region Public Properties
        /// <summary>
        ///     Gets or sets the padding count.
        /// </summary>
        public int PaddingCount { get; set; }

        /// <summary>
        ///     Gets or sets the length of the padding.
        /// </summary>
        public int PaddingLength { get; set; } = 4;
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the padding.
        /// </summary>
        string Padding
        {
            get { return "".PadRight(PaddingLength * PaddingCount); }
        }
        #endregion

        #region Public Methods
        /// <summary>
        ///     Appends the specified value.
        /// </summary>
        public void Append(string value)
        {
            sb.Append(value);
        }

        /// <summary>
        ///     Appends the line.
        /// </summary>
        public void AppendLine(string line)
        {
            sb.Append(Padding);
            sb.AppendLine(line);
        }

        /// <summary>
        ///     Appends the with padding.
        /// </summary>
        public void AppendWithPadding(string value)
        {
            sb.Append(Padding);
            sb.Append(value);
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        public override string ToString()
        {
            return sb.ToString();
        }
        #endregion
    }
}
using System;

namespace Transporter.Core.Utils;

public class LocalDateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.Now;
}
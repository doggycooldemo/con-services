﻿using System;
using System.ComponentModel.DataAnnotations;

namespace VSS.Productivity3D.Productivity3D.Models.Validation
{
  /// <summary>
  /// Validates the passed project ID.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public class ValidProjectIdAttribute : ValidationAttribute
  {
    /// <summary>
    /// Validates the specified value with respect to the current validation attribute.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="validationContext">The context information about the validation operation.</param>
    /// <returns>An instance of the <see cref="T:System.ComponentModel.DataAnnotations.ValidationResult" /> class.</returns>
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      try
      {
        // Id's can be negative, but not 0 or -1
        var v = (long?) value;
        if (!((long?) value).HasValue || !(v == 0 || v == -1))
        {
          return ValidationResult.Success;
        }
      }
      catch (InvalidCastException)
      {
        // Consume any type casting errors and allow the function to return a failed result.
      }

      return new ValidationResult("Invalid project ID");
    }
  }
}

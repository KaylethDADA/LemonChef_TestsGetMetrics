﻿using Domain.Entities;
using Domain.Validations.Primitives;
using FluentValidation;

namespace Domain.Validations.Validators;
public class RecipeValidator : AbstractValidator<Recipe>
{
    public RecipeValidator(string paramName)
    {
        RuleFor(param => param.Title)
            .NotNullOrEmptyWithMessage(nameof(Recipe.Title))
            .Length(1,250)
            .WithMessage(ExceptionMessages.InvalidFormat(nameof(Recipe.Title)));

        RuleFor(param => param.Link)
            .IsValidUrlWithMessage(nameof(Recipe.Link))
            .When(param=> param.Link is not null);
        
        RuleForEach(param => param.Ingredients);

        RuleFor(param => param.Description)
            .NotNullOrEmptyWithMessage(nameof(Recipe.Description))
            .Length(1,50000)
            .WithMessage(ExceptionMessages.InvalidFormat(nameof(Recipe.Description)));
        
        RuleFor(param=> param.Servings)
            .GreaterThan(0)
            .When(param=> param.Servings is not null)
            .WithMessage(ExceptionMessages.TooLowNumber(nameof(Recipe.Servings)));
        
        RuleFor(param=>param.PreparationTime)
            .GreaterThan(0)
            .When(param=> param.PreparationTime is not null)
            .WithMessage(ExceptionMessages.TooLowNumber(nameof(Recipe.PreparationTime)));
        
        RuleFor(param => param.TelegramUserId)
            .NotNullOrEmptyWithMessage(nameof(Recipe.TelegramUserId))
            .GreaterThan(0)
            .WithMessage(ExceptionMessages.InvalidFormat(nameof(Recipe.TelegramUserId)));
    }
}
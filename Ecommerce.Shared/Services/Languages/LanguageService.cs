using Ecommerce.Shared.Context;
using Ecommerce.Shared.Entities;
using Ecommerce.Shared.Services.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Services.Languages;

public interface ILanguageService
    {
    Task<ServiceResponse<List<Language>>> GetLanguagesAsync();
    Task<ServiceResponse<Language>> GetLanguageByIdAsync(int id);
    Task<ServiceResponse<Language>> AddLanguageAsync(Language language);
    Task<ServiceResponse<Language>> UpdateLanguageAsync(Language language);
    Task<ServiceResponse<bool>> DeleteLanguageAsync(int id);
    }
public class LanguageService : ILanguageService
    {
    private readonly ILogger<LanguageService> _logger;
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public LanguageService(IDbContextFactory<ApplicationDbContext> contextFactory, ILogger<LanguageService> logger)
        {
        _contextFactory = contextFactory;
        _logger = logger;
        }

    public async Task<ServiceResponse<List<Language>>> GetLanguagesAsync()
        {
        try
            {
            using var _context = _contextFactory.CreateDbContext();
            var languages = await _context.Languages.ToListAsync();
            return new ServiceResponse<List<Language>>
                {
                Data = languages,
                Success = true,
                Message = "Languages fetched successfully"
                };
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while fetching languages.");
            return new ServiceResponse<List<Language>>
                {
                Success = false,
                Message = "Error occurred while fetching languages"
                };
            }
        }

    public async Task<ServiceResponse<Language>> GetLanguageByIdAsync(int id)
        {
        try
            {
            using var _context = _contextFactory.CreateDbContext();
            var language = await _context.Languages.FindAsync(id);
            if (language == null)
                {
                return new ServiceResponse<Language>
                    {
                    Success = false,
                    Message = "Language not found"
                    };
                }
            return new ServiceResponse<Language>
                {
                Data = language,
                Success = true,
                Message = "Language fetched successfully"
                };
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while fetching language by ID.");
            return new ServiceResponse<Language>
                {
                Success = false,
                Message = "Error occurred while fetching language by ID"
                };
            }
        }

    public async Task<ServiceResponse<Language>> AddLanguageAsync(Language language)
        {
        try
            {
            using var _context = _contextFactory.CreateDbContext();
            _context.Languages.Add(language);
            await _context.SaveChangesAsync();

            return new ServiceResponse<Language>
                {
                Data = language,
                Success = true,
                Message = "Language added successfully"
                };
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while adding language.");
            return new ServiceResponse<Language>
                {
                Success = false,
                Message = "Error occurred while adding language"
                };
            }
        }

    public async Task<ServiceResponse<Language>> UpdateLanguageAsync(Language updatedLanguage)
        {
        try
            {
            using var _context = _contextFactory.CreateDbContext();
            var existingLanguage = await _context.Languages.FindAsync(updatedLanguage.Id);

            if (existingLanguage == null)
                {
                return new ServiceResponse<Language>
                    {
                    Success = false,
                    Message = "Language not found"
                    };
                }

            existingLanguage.LanguageCode = updatedLanguage.LanguageCode;
            existingLanguage.LanguageName = updatedLanguage.LanguageName;
            _context.Languages.Update(existingLanguage);
            await _context.SaveChangesAsync();

            return new ServiceResponse<Language>
                {
                Data = existingLanguage,
                Success = true,
                Message = "Language updated successfully"
                };
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while updating language.");
            return new ServiceResponse<Language>
                {
                Success = false,
                Message = "Error occurred while updating language"
                };
            }
        }

    public async Task<ServiceResponse<bool>> DeleteLanguageAsync(int id)
        {
        try
            {
            using var _context = _contextFactory.CreateDbContext();
            var language = await _context.Languages.FindAsync(id);
            if (language == null)
                {
                return new ServiceResponse<bool>
                    {
                    Success = false,
                    Message = "Language not found"
                    };
                }

            _context.Languages.Remove(language);
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool>
                {
                Data = true,
                Success = true,
                Message = "Language deleted successfully"
                };
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while deleting language.");
            return new ServiceResponse<bool>
                {
                Success = false,
                Message = "Error occurred while deleting language"
                };
            }
        }
    }


using CommentApp.Domain.Enums;

namespace CommentApp.Application.Dtos;
public record FileDto(string FilePath, FileType FileType);
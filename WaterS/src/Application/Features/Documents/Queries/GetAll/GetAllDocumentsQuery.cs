﻿using MediatR;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using WaterS.Application.Extensions;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Application.Interfaces.Services;
using WaterS.Application.Specifications.Misc;
using WaterS.Domain.Entities.Misc;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Features.Documents.Queries.GetAll
{
    public class GetAllDocumentsQuery : IRequest<PaginatedResult<GetAllDocumentsResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchString { get; set; }

        public GetAllDocumentsQuery(int pageNumber, int pageSize, string searchString)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            SearchString = searchString;
        }
    }

    internal class GetAllDocumentsQueryHandler : IRequestHandler<GetAllDocumentsQuery, PaginatedResult<GetAllDocumentsResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;

        private readonly ICurrentUserService _currentUserService;

        public GetAllDocumentsQueryHandler(IUnitOfWork<int> unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<PaginatedResult<GetAllDocumentsResponse>> Handle(GetAllDocumentsQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Document, GetAllDocumentsResponse>> expression = e => new GetAllDocumentsResponse
            {
                Id = e.Id,
                Title = e.Title,
                CreatedBy = e.CreatedBy,
                IsPublic = e.IsPublic,
                CreatedOn = e.CreatedOn,
                Description = e.Description,
                URL = e.URL,
                DocumentType = e.DocumentType.Name,
                DocumentTypeId = e.DocumentTypeId
            };
            var docSpec = new DocumentFilterSpecification(request.SearchString, _currentUserService.UserId);
            var data = await _unitOfWork.Repository<Document>().Entities
               .Specify(docSpec)
               .Select(expression)
               .ToPaginatedListAsync(request.PageNumber, request.PageSize);
            return data;
        }
    }
}
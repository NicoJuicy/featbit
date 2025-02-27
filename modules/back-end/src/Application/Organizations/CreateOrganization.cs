using Application.Bases;
using Application.Bases.Exceptions;
using Domain.Organizations;
using Domain.Policies;

namespace Application.Organizations;

public class CreateOrganization : IRequest<OrganizationVm>
{
    public Guid WorkspaceId { get; set; }

    public string Name { get; set; }

    public string Key { get; set; }

    public Guid CurrentUserId { get; set; }
}

public class CreateOrganizationValidator : AbstractValidator<CreateOrganization>
{
    public CreateOrganizationValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithErrorCode(ErrorCodes.Required("name"));

        RuleFor(x => x.Key)
            .NotEmpty().WithErrorCode(ErrorCodes.Required("key"));
    }
}

public class CreateOrganizationHandler : IRequestHandler<CreateOrganization, OrganizationVm>
{
    private readonly IOrganizationService _service;
    private readonly IMapper _mapper;

    public CreateOrganizationHandler(IOrganizationService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    public async Task<OrganizationVm> Handle(CreateOrganization request, CancellationToken cancellationToken)
    {
        var keyHasBeenUsed = await _service.HasKeyBeenUsedAsync(request.WorkspaceId, request.Key);
        if (keyHasBeenUsed)
        {
            throw new BusinessException(ErrorCodes.KeyHasBeenUsed);
        }

        // add new organization
        var organization = new Organization(request.WorkspaceId, request.Name, request.Key);
        await _service.AddOneAsync(organization);

        // add user to organization
        var organizationUser = new OrganizationUser(organization.Id, request.CurrentUserId);
        var policies = new[] { BuiltInPolicy.Owner };
        await _service.AddUserAsync(organizationUser, policies: policies);

        return _mapper.Map<OrganizationVm>(organization);
    }
}
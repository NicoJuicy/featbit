﻿using Application.Resources;
using Domain.Organizations;
using Domain.Projects;
using Domain.Resources;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Environment = Domain.Environments.Environment;

namespace Infrastructure.Services.MongoDb;

public class ResourceService(MongoDbClient mongoDb) : IResourceService
{
    public async Task<IEnumerable<Resource>> GetResourcesAsync(Guid organizationId, ResourceFilter filter)
    {
        var name = filter.Name;

        return filter.Type switch
        {
            ResourceTypes.All => [Resource.All],
            ResourceTypes.Workspace => [Resource.AllWorkspace],
            ResourceTypes.Organization => [Resource.AllOrganizations],
            ResourceTypes.Iam => [Resource.AllIam],
            ResourceTypes.AccessToken => [Resource.AllAccessToken],
            ResourceTypes.RelayProxy => [Resource.AllRelayProxies],
            ResourceTypes.FeatureFlag => [Resource.AllFeatureFlag],
            ResourceTypes.Segment => [Resource.AllSegments],
            ResourceTypes.Env => await GetEnvsAsync(organizationId, name),
            ResourceTypes.Project => await GetProjectsAsync(organizationId, name),
            _ => Array.Empty<Resource>()
        };
    }

    public async Task<IEnumerable<Resource>> GetProjectsAsync(Guid organizationId, string name)
    {
        var query = mongoDb.QueryableOf<Project>()
            .Where(project => project.OrganizationId == organizationId)
            .Select(project => new
            {
                project.Id,
                project.Name,
                Rn = "project/" + project.Key
            });
        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(x => x.Name.Contains(name, StringComparison.CurrentCultureIgnoreCase));
        }

        var items = await query.ToListAsync();

        var resources = items.Select(x => new Resource
        {
            Id = x.Id,
            Name = x.Name,
            Rn = x.Rn,
            Type = ResourceTypes.Project
        }).ToList();

        resources.Insert(0, Resource.AllProject);
        return resources;
    }

    public async Task<IEnumerable<Resource>> GetEnvsAsync(Guid organizationId, string name)
    {
        var organizations = mongoDb.QueryableOf<Organization>();
        var projects = mongoDb.QueryableOf<Project>();
        var envs = mongoDb.QueryableOf<Environment>();

        var query =
            from env in envs
            join project in projects on env.ProjectId equals project.Id
            join organization in organizations on project.OrganizationId equals organization.Id
            where organization.Id == organizationId
            select new
            {
                env.Id,
                env.Name,
                Rn = "project/" + project.Key + ":env/" + env.Key
            };

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(x => x.Name.Contains(name, StringComparison.CurrentCultureIgnoreCase));
        }

        var items = await query.ToListAsync();

        var resources = items.Select(x => new Resource
        {
            Id = x.Id,
            Name = x.Name,
            Rn = x.Rn,
            Type = ResourceTypes.Env
        }).ToList();

        resources.Insert(0, Resource.AllProjectEnv);
        return resources;
    }
}
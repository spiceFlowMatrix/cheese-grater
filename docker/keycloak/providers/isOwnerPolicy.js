var context = $evaluation.getContext();
var permission = $evaluation.getPermission();

isOwnerPolicy(permission, context);

/**
 * Policy: Is Owner
 * Description: Checks if the requesting user is the owner of the resource.
 * Assumes the resource has an attribute 'owner' containing the user ID.
 */
function isOwnerPolicy(permission, context) {
    print("Starting isOwnerPolicy evaluation");

    var identity = context.identity;
    var resource = permission.resource;

    if (!identity || !resource) {
        print("Missing identity or resource: identity=" + identity + ", resource=" + resource);
        $evaluation.deny();
        return;
    }

    var resourceOwnerId = resource.get('owner');
    var requestingUserId = identity.getId();

    print("Resource Owner ID: " + resourceOwnerId);
    print("Requesting User ID: " + requestingUserId);

    if (resourceOwnerId && requestingUserId && resourceOwnerId === requestingUserId) {
        print("User is the owner, granting access");
        $evaluation.grant();
    } else {
        print("User is not the owner or IDs are missing, denying access");
        $evaluation.deny();
    }
}
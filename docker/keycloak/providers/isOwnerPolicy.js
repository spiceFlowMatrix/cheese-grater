/**
 * Policy: Is Owner
 * Description: Checks if the requesting user is the owner of the resource.
 * Assumes the resource has an attribute 'owner' containing the user ID.
 */
function isOwnerPolicy(permission, context) {
    var identity = context.identity;
    var resource = permission.resource;

    if (!identity || !resource) {
        // If identity or resource is missing, policy cannot evaluate
        return false;
    }

    var resourceOwnerId = resource.get('owner'); // Get the 'owner' attribute from the resource
    var requestingUserId = identity.get('id');   // Get the ID of the requesting user

    if (resourceOwnerId && requestingUserId && resourceOwnerId === requestingUserId) {
        $evaluation.grant(); // Grant access if the user is the owner
        return true;
    }

    $evaluation.deny(); // Deny access otherwise
    return false;
}
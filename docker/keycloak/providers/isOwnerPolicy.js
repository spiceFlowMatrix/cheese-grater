var context = $evaluation.getContext();
var permission = $evaluation.getPermission();

isOwnerPolicy(permission, context);

/**
 * Policy: Is Owner
 * Description: Checks if the requesting user is the owner of the resource.
 * Assumes the resource has an attribute 'owner' containing the user ID.
 */
function isOwnerPolicy(permission, context) {
  var identity = context.identity;
  var resource = permission.getResource();

  if (!identity || !resource) {
    $evaluation.deny();
    return;
  }

  var resourceOwnerAttribute = resource.getAttribute(identity.getId());

  if (
    resourceOwnerAttribute &&
    (resourceOwnerAttribute.contains('Owner') ||
      resourceOwnerAttribute.contains('owner'))
  ) {
    $evaluation.grant();
  } else {
    $evaluation.deny();
  }
}

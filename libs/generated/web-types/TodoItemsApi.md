# .TodoItemsApi

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**createTodoItem**](TodoItemsApi.md#createTodoItem) | **POST** /api/TodoItems | 
[**deleteTodoItem**](TodoItemsApi.md#deleteTodoItem) | **DELETE** /api/TodoItems/{id} | 
[**getTodoItemsWithPagination**](TodoItemsApi.md#getTodoItemsWithPagination) | **GET** /api/TodoItems | 
[**updateTodoItem**](TodoItemsApi.md#updateTodoItem) | **PUT** /api/TodoItems/{id} | 
[**updateTodoItemDetail**](TodoItemsApi.md#updateTodoItemDetail) | **PUT** /api/TodoItems/UpdateDetail/{id} | 


# **createTodoItem**
> number createTodoItem(createTodoItemCommand)


### Example


```typescript
import { createConfiguration, TodoItemsApi } from '';
import type { TodoItemsApiCreateTodoItemRequest } from '';

const configuration = createConfiguration();
const apiInstance = new TodoItemsApi(configuration);

const request: TodoItemsApiCreateTodoItemRequest = {
  
  createTodoItemCommand: {
    listId: 1,
    title: "title_example",
  },
};

const data = await apiInstance.createTodoItem(request);
console.log('API called successfully. Returned data:', data);
```


### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **createTodoItemCommand** | **CreateTodoItemCommand**|  |


### Return type

**number**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
**201** |  |  -  |

[[Back to top]](#) [[Back to API list]](README.md#documentation-for-api-endpoints) [[Back to Model list]](README.md#documentation-for-models) [[Back to README]](README.md)

# **deleteTodoItem**
> void deleteTodoItem()


### Example


```typescript
import { createConfiguration, TodoItemsApi } from '';
import type { TodoItemsApiDeleteTodoItemRequest } from '';

const configuration = createConfiguration();
const apiInstance = new TodoItemsApi(configuration);

const request: TodoItemsApiDeleteTodoItemRequest = {
  
  id: 1,
};

const data = await apiInstance.deleteTodoItem(request);
console.log('API called successfully. Returned data:', data);
```


### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **id** | [**number**] |  | defaults to undefined


### Return type

**void**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
**204** |  |  -  |

[[Back to top]](#) [[Back to API list]](README.md#documentation-for-api-endpoints) [[Back to Model list]](README.md#documentation-for-models) [[Back to README]](README.md)

# **getTodoItemsWithPagination**
> PaginatedListOfTodoItemBriefDto getTodoItemsWithPagination()


### Example


```typescript
import { createConfiguration, TodoItemsApi } from '';
import type { TodoItemsApiGetTodoItemsWithPaginationRequest } from '';

const configuration = createConfiguration();
const apiInstance = new TodoItemsApi(configuration);

const request: TodoItemsApiGetTodoItemsWithPaginationRequest = {
  
  listId: 1,
  
  pageNumber: 1,
  
  pageSize: 1,
};

const data = await apiInstance.getTodoItemsWithPagination(request);
console.log('API called successfully. Returned data:', data);
```


### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **listId** | [**number**] |  | defaults to undefined
 **pageNumber** | [**number**] |  | defaults to undefined
 **pageSize** | [**number**] |  | defaults to undefined


### Return type

**PaginatedListOfTodoItemBriefDto**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** |  |  -  |

[[Back to top]](#) [[Back to API list]](README.md#documentation-for-api-endpoints) [[Back to Model list]](README.md#documentation-for-models) [[Back to README]](README.md)

# **updateTodoItem**
> void updateTodoItem(updateTodoItemCommand)


### Example


```typescript
import { createConfiguration, TodoItemsApi } from '';
import type { TodoItemsApiUpdateTodoItemRequest } from '';

const configuration = createConfiguration();
const apiInstance = new TodoItemsApi(configuration);

const request: TodoItemsApiUpdateTodoItemRequest = {
  
  id: 1,
  
  updateTodoItemCommand: {
    id: 1,
    title: "title_example",
    done: true,
  },
};

const data = await apiInstance.updateTodoItem(request);
console.log('API called successfully. Returned data:', data);
```


### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **updateTodoItemCommand** | **UpdateTodoItemCommand**|  |
 **id** | [**number**] |  | defaults to undefined


### Return type

**void**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
**204** |  |  -  |
**400** |  |  -  |

[[Back to top]](#) [[Back to API list]](README.md#documentation-for-api-endpoints) [[Back to Model list]](README.md#documentation-for-models) [[Back to README]](README.md)

# **updateTodoItemDetail**
> void updateTodoItemDetail(updateTodoItemDetailCommand)


### Example


```typescript
import { createConfiguration, TodoItemsApi } from '';
import type { TodoItemsApiUpdateTodoItemDetailRequest } from '';

const configuration = createConfiguration();
const apiInstance = new TodoItemsApi(configuration);

const request: TodoItemsApiUpdateTodoItemDetailRequest = {
  
  id: 1,
  
  updateTodoItemDetailCommand: {
    id: 1,
    listId: 1,
    priority: 0,
    note: "note_example",
  },
};

const data = await apiInstance.updateTodoItemDetail(request);
console.log('API called successfully. Returned data:', data);
```


### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **updateTodoItemDetailCommand** | **UpdateTodoItemDetailCommand**|  |
 **id** | [**number**] |  | defaults to undefined


### Return type

**void**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
**204** |  |  -  |
**400** |  |  -  |

[[Back to top]](#) [[Back to API list]](README.md#documentation-for-api-endpoints) [[Back to Model list]](README.md#documentation-for-models) [[Back to README]](README.md)



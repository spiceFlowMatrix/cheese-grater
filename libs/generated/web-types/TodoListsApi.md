# .TodoListsApi

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**createTodoList**](TodoListsApi.md#createTodoList) | **POST** /api/TodoLists | 
[**deleteTodoList**](TodoListsApi.md#deleteTodoList) | **DELETE** /api/TodoLists/{id} | 
[**getTodoLists**](TodoListsApi.md#getTodoLists) | **GET** /api/TodoLists | 
[**updateTodoList**](TodoListsApi.md#updateTodoList) | **PUT** /api/TodoLists/{id} | 


# **createTodoList**
> TodoListDto createTodoList(createTodoListCommand)


### Example


```typescript
import { createConfiguration, TodoListsApi } from '';
import type { TodoListsApiCreateTodoListRequest } from '';

const configuration = createConfiguration();
const apiInstance = new TodoListsApi(configuration);

const request: TodoListsApiCreateTodoListRequest = {
  
  createTodoListCommand: {
    title: "title_example",
  },
};

const data = await apiInstance.createTodoList(request);
console.log('API called successfully. Returned data:', data);
```


### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **createTodoListCommand** | **CreateTodoListCommand**|  |


### Return type

**TodoListDto**

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

# **deleteTodoList**
> void deleteTodoList()


### Example


```typescript
import { createConfiguration, TodoListsApi } from '';
import type { TodoListsApiDeleteTodoListRequest } from '';

const configuration = createConfiguration();
const apiInstance = new TodoListsApi(configuration);

const request: TodoListsApiDeleteTodoListRequest = {
  
  id: 1,
};

const data = await apiInstance.deleteTodoList(request);
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

# **getTodoLists**
> TodosVm getTodoLists()


### Example


```typescript
import { createConfiguration, TodoListsApi } from '';

const configuration = createConfiguration();
const apiInstance = new TodoListsApi(configuration);

const request = {};

const data = await apiInstance.getTodoLists(request);
console.log('API called successfully. Returned data:', data);
```


### Parameters
This endpoint does not need any parameter.


### Return type

**TodosVm**

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

# **updateTodoList**
> TodoListDto updateTodoList(updateTodoListCommand)


### Example


```typescript
import { createConfiguration, TodoListsApi } from '';
import type { TodoListsApiUpdateTodoListRequest } from '';

const configuration = createConfiguration();
const apiInstance = new TodoListsApi(configuration);

const request: TodoListsApiUpdateTodoListRequest = {
  
  id: 1,
  
  updateTodoListCommand: {
    id: 1,
    title: "title_example",
  },
};

const data = await apiInstance.updateTodoList(request);
console.log('API called successfully. Returned data:', data);
```


### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **updateTodoListCommand** | **UpdateTodoListCommand**|  |
 **id** | [**number**] |  | defaults to undefined


### Return type

**TodoListDto**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** |  |  -  |
**400** |  |  -  |

[[Back to top]](#) [[Back to API list]](README.md#documentation-for-api-endpoints) [[Back to Model list]](README.md#documentation-for-models) [[Back to README]](README.md)



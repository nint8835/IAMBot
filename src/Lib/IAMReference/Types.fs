namespace IAMBot.IAMReference

open FSharp.Json

type Condition =
    { [<JsonField("condition")>]
      Condition: string
      [<JsonField("description")>]
      Description: string
      [<JsonField("type")>]
      // TODO: Enum?
      Type: string }

type ResourceType =
    { [<JsonField("condition_keys")>]
      ConditionKeys: string[]
      [<JsonField("resource_type")>]
      ResourceType: string
      [<JsonField("dependent_actions")>]
      DependentActions: string[] }

type Resource =
    { [<JsonField("arn")>]
      Arn: string
      [<JsonField("condition_keys")>]
      ConditionKeys: string[]
      [<JsonField("resource")>]
      Resource: string }

type Action =
    { [<JsonField("access_level")>]
      // TODO: Enum?
      AccessLevel: string
      [<JsonField("description")>]
      Description: string
      [<JsonField("privilege")>]
      Action: string
      [<JsonField("resource_types")>]
      ResourceTypes: ResourceType[] }

type Service =
    { [<JsonField("prefix")>]
      Prefix: string
      [<JsonField("conditions")>]
      Conditions: Condition[]
      [<JsonField("resources")>]
      Resources: Resource[]
      [<JsonField("privileges")>]
      Actions: Action[]
      [<JsonField("service_name")>]
      ServiceName: string }

type ReferenceFile = Service[]

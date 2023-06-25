namespace IAMBot.IAMReference

open FSharp.Json

type Condition =
    { Condition: string
      Description: string
      // TODO: Enum?
      Type: string }

type ResourceType =
    { ConditionKeys: string[]
      ResourceType: string
      DependentActions: string[] }

type Resource =
    { Arn: string
      ConditionKeys: string[]
      Resource: string }

type Action =
    { // TODO: Enum?
      AccessLevel: string
      Description: string
      [<JsonField("privilege")>]
      Action: string
      ResourceTypes: ResourceType[] }

type Service =
    { // TODO: Enum?
      Prefix: string
      Conditions: Condition[]
      Resources: Resource[]
      [<JsonField("privileges")>]
      Actions: Action[]
      ServiceName: string }

type ReferenceFile = Service[]

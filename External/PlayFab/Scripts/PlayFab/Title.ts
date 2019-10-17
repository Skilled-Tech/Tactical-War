namespace PlayFab
{
    export namespace Title
    {
        export namespace Data
        {
            export function RetrieveAll(keys: string[]): PlayFabServerModels.GetTitleDataResult
            {
                let result = server.GetTitleData({
                    Keys: keys,
                })

                return result;
            }

            export function Retrieve(key: string): string | null
            {
                var result = RetrieveAll([key]);

                if (result.Data == null) return null;

                if (result.Data[key] == null) return null;

                return result.Data[key];
            }
        }
    }
}
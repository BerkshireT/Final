from graphqlclient import GraphQLClient, json
import sys
import Key

perPage = sys.argv[1]
coordinates = sys.argv[2]
radius = sys.argv[3]

authToken = Key.apiKey
apiVersion = 'alpha'
client = GraphQLClient('https://api.smash.gg/gql/' + apiVersion)
client.inject_token('Bearer ' + authToken)

result = client.execute('''
query TournamentsByLocation($perPage: Int, $coordinates: String!, $radius: String!) {
  tournaments(query: {
    perPage: $perPage
    filter: {
      location: {
        distanceFrom: $coordinates,
        distance: $radius
      },
      past: false,
      videogameIds: [
        1, 2, 3, 4, 5
      ]
    }
  }) {
    nodes {
      id
      name
      city
      addrState
      startAt
      venueAddress
      images(type: "profile") {
        url
      }
      slug
      primaryContact
      events {
        name
        numEntrants
      }
    }
  }
}
''',
{
    "perPage": perPage,
    "coordinates": coordinates,
    "radius": radius
})

resData = json.loads(result)
f = open("./public/tournaments.json", "w")

if 'errors' in resData:
    f.write('Error:')
    f.write(json.dumps(resData['errors']))
else:
    del resData['extensions']
    for tournie in resData["data"]["tournaments"]["nodes"] :
        imageURL = tournie["images"][0]["url"]
        del tournie["images"]
        tournie.update({"imageURL" : imageURL})

        if tournie["primaryContact"] is None:
            tournie["primaryContact"] = "None"

        for event in tournie["events"]:
            if event["numEntrants"] is None:
                event["numEntrants"] = 0

    f.write(json.dumps(resData["data"]["tournaments"]["nodes"]))
f.close()

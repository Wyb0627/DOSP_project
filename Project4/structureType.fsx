module structureType


open System.Collections.Generic
open System


type tweet =
    struct
        val mutable content: string
        val mutable tweet_owner: string
        val mutable retweet_list: List<string>
        val mutable hashtags: List<string>
        val mutable Ats: List<string>
        val mutable likes: int
        val mutable isRetweet: bool

        new(tweet_owner: string, content: string) =
            { tweet_owner = tweet_owner
              content = content
              likes = 0
              retweet_list = new List<string>()
              Ats = new List<string>()
              hashtags = new List<string>()
              isRetweet = false
               }

        member this.LikeTweet(tweet: tweet) = this.likes <- this.likes + 1
        member this.retweet(rewteetUser: string) = this.retweet_list.Add(rewteetUser)

        member this.FindTagandAt() =
            let len = String.length this.content
            for i = 0 to len - 1 do
                if this.content.[i] = '#' then
                    let mutable j = i
                    while j < len && this.content.[j] <> ' ' do
                        j <- j + 1
                    this.hashtags.Add(this.content.[i..j-1])
                elif this.content.[i] = '@' then
                    let mutable j = i
                    while j < len && this.content.[j] <> ' ' do
                        j <- j + 1
                    this.Ats.Add(this.content.[i..j-1])

    end



type tweetList =
    struct
        val mutable userName: string
        val mutable tweetsList: List<tweet>

        new(userName: string) =
            { userName = userName
              tweetsList = new List<tweet>() }
    end

type tweetMessage =
    struct
        val mutable userName: string
        val mutable tweet: tweet
        val mutable isRetweet: bool
        new(userName: string, tweet: tweet, isRetweet: bool) =
            { userName = userName
              tweet = tweet
              isRetweet = isRetweet }
    end



type user =
    struct
        val mutable userName: string
        val mutable TweetList: List<tweet>
        val mutable Subscribers: List<string>
        val mutable Subscribe: List<string>

        new(userName: string) =
            { userName = userName
              TweetList = new List<tweet>()
              Subscribers = new List<string>() //粉丝
              Subscribe = new List<string>() } //关注的人

        member this.follow(user: user) =
            this.Subscribe.Add(user.userName)
            user.Subscribers.Add(this.userName)

        member this.WriteTwiter(content: string) =
            this.TweetList.Add(tweet (this.userName, content))

        member this.retweet(tweet: tweet) = this.TweetList.Add(tweet)
    end

type followMessage =
    struct
        val mutable user: string
        val mutable followUser: string

        new(user: string, followUser: string) = { user = user; followUser = followUser }
    end


type querySub =
    struct
        val mutable userName: string
        new(userName: string) = { userName = userName }
    end

type queryTag =
    struct
        val mutable tag: string
        val mutable userName: string
        new(tag: string, userName: string) = { tag = tag; userName = userName }
    end

type queryAt =
    struct
        val mutable at: string
        val mutable userName: string
        new(at: string, userName: string) = { at = at; userName = userName }
    end

type test =
    struct
        val mutable message: string
        new(message: string) = { message = message}
    end

type link =
    struct
        val mutable userName: string
        new(userName: string) = { userName = userName}
    end

type unlink =
    struct
        val mutable userName: string
        new(userName: string) = { userName = userName}
    end

type timer =
    struct
    end

type endtimer =
    struct
    end
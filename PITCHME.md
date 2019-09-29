# @css[az](WTF) @color[grey](GitHub)
## ... don't take me @color[orange](so seriously)

---

<img class="rounded" src="assets/me.png" height="420" />

@snap[north span-80]
@emoji[wave text-14] @css[text-14](Hi, I'm David Pine)
@snapend

---

<img class="rounded" src="assets/me.png" height="420" />

@snap[south-west]
[@css[twitter](@fa[twitter]) @davidpine7](https://twitter.com/davidpine7)
[@color[#f5f5f5](@fab[github]) github.com/IEvangelist](https://github.com/IEvangelist)
@snapend

@snap[west clear fit]
<a href="https://fallexperiment.com/creamcitycode" target="_blank">
    <img  class="clear fit" src="assets/creamcitycode.png" />
</a>
@snapend

@snap[north-west clear fit]
    <img src="assets/mvp.png" />
@snapend

@snap[south-east]
[davidpine.net @color[red](@fa[globe])](https://davidpine.net/)
[docs.microsoft.com @color[#008AD7](@fab[microsoft])](https://docs.microsoft.com/azure)
@snapend

@snap[east clear fit]
    <img src="assets/twilio-mark-red.png" />
@snapend

@snap[north-east clear fit]
    <img src="assets/gde.png" />
@snapend

---

# @color[white](I) @color[red](@fa[heart]) @color[white](The)
### #DeveloperCommunity @css[twitter](@fa[twitter])

---?image=profanity-filter.png&size=contain
---?image=assets/slide-one.png&size=contain
---

# GitHub @color[grey](@fab[github])
# @color[green](Webhooks)

---

```cs zoom-11
public bool IsPayloadSignatureValid(
	byte[] bytes, 
	string receivedSignature)
{
    if (string.IsNullOrWhiteSpace(receivedSignature))
    {
        return false;
    }

    using var hmac = new HMACSHA1(
		Encoding.ASCII.GetBytes(_options.WebhookSecret));

    var hash = hmac.ComputeHash(bytes);
    var actualSignature = $"sha1={hash.ToHexString()}";

    return IsSignatureValid(
		actualSignature, receivedSignature);
}
```

@snap[south span-100]
@[1-3](Method signature, `bool` return, parameters `byte[]` and signature)
@[5-8](Early exit)
@[10-11](C# 8, simplified `using`)
@[13-17](Compute the hash and call `IsSignatureValid`)
@snapend

---

```cs zoom-14
static bool IsSignatureValid(
	string a,
	string b)
{
    var length = Math.Min(a.Length, b.Length);
    var equals = a.Length == b.Length;
    for (var i = 0; i < length; ++ i)
    {
        equals &= a[i] == b[i];
    }

    return equals;
}
```

@snap[south span-100]
@[1-3](A `bool` return, two `string` parameters)
@[5](Determine the shortest `length`)
@[6](Declare and assign `equals`)
@[7-12](Compare each `char` in both `string` instances for equality)
@snapend

---

```cs zoom-13
public ValueTask DispatchAsync(
	string eventName,
	string payloadJson) => 
	eventName switch
    {
        "issues" => 
			_issueHandler.HandleIssueAsync(
				payloadJson),
        "pull_request" => 
			_pullHandler.HandlePullRequestAsync(
				payloadJson),

        _ => new ValueTask(),
    };
```

@snap[south span-100]
@[1-3](A `ValueTask` return, `eventName` and JSON parameters)
@[4-5,14](C# 8, `switch` expressions)
@[6-8](Handle `issues`)
@[9-11](Handle `pull requests`)
@[13](Handle `default` case, "catch-all")
@snapend

---

# GitHub @color[grey](@fab[github])
# @color[red](Labels)

---

## [Labels @fa[external-link-alt]](https://github.com/IEvangelist/GitHub.ProfanityFilter/labels)

---

# GitHub @color[grey](@fab[github])
# @color[magenta](GraphQL)

---

```js zoom-17
query {
  repository(
    owner: "IEvangelist", 
    name: "GitHub.ProfanityFilter") {
    labels(first: 20) {
      nodes { id, name }
    }
  }
}
```

@snap[south span-100]
@[1](We start with a `query` or a `mutation`)
@[2-8](We're going to `query` a `repository`)
@[3-4](Given a `name` and `owner`)
@[5](Select the `first` 20 `labels`)
@[6](Take the `id` and `name` from the `nodes`)
@snapend

---

![Lint Licker](https://www.youtube.com/embed/sf4VC-xNsP8)

---

# @color[grey](@fab[github]) Source @color[cyan](@fa[code])
<br>
## [bit.ly/ProfanityFilter](https://bit.ly/ProfanityFilter)

---

<img class="rounded" src="assets/me.png" height="420" />

@snap[south-west]
[@css[twitter](@fa[twitter]) @davidpine7](https://twitter.com/davidpine7)
[@color[#f5f5f5](@fab[github]) github.com/IEvangelist](https://github.com/IEvangelist)
@snapend

@snap[north]
@emoji[clap text-14] @css[text-14](Thank You)
@snapend

@snap[south-east]
[davidpine.net @color[red](@fa[globe])](https://davidpine.net/)
[docs.microsoft.com @color[#008AD7](@fab[microsoft])](https://docs.microsoft.com/azure)
@snapend
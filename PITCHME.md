# @css[az](WTF) @color[grey](GitHub)
### ... don't take me @color[orange](so seriously)

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
## @color[orange](Webhooks)

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

---

# GitHub @color[grey](@fab[github])
## @color[orange](GraphQL)

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
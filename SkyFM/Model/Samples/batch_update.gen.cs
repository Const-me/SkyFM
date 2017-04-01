public class Asset
{
    public int id { get; set; }
    public string name { get; set; }
    public string content_hash { get; set; }
}

public class Images
{
    public string @default { get; set; }
}

public class Channel
{
    public string ad_channels { get; set; }
    public string channel_director { get; set; }
    public string created_at { get; set; }
    public string description_long { get; set; }
    public string description_short { get; set; }
    public int? forum_id { get; set; }
    public int id { get; set; }
    public string key { get; set; }
    public string name { get; set; }
    public int network_id { get; set; }
    public int? old_id { get; set; }
    public int? premium_id { get; set; }
    public int tracklist_server_id { get; set; }
    public string updated_at { get; set; }
    public int asset_id { get; set; }
    public string asset_url { get; set; }
    public object banner_url { get; set; }
    public string description { get; set; }
    public List<object> similar_channels { get; set; }
    public Images images { get; set; }
}

public class ChannelFilter
{
    public bool display { get; set; }
    public int id { get; set; }
    public string key { get; set; }
    public bool meta { get; set; }
    public string name { get; set; }
    public int network_id { get; set; }
    public int position { get; set; }
    public string sprite { get; set; }
    public List<Channel> channels { get; set; }
}

public class Stream
{
    public int id { get; set; }
    public string url { get; set; }
    public string format { get; set; }
    public int bitrate { get; set; }
}

public class Channel2
{
    public int id { get; set; }
    public List<Stream> streams { get; set; }
}

public class Streamlist
{
    public int id { get; set; }
    public string name { get; set; }
    public List<Channel2> channels { get; set; }
}

public class StreamSet
{
    public int id { get; set; }
    public int network_id { get; set; }
    public string name { get; set; }
    public string key { get; set; }
    public string description { get; set; }
    public Streamlist streamlist { get; set; }
}

public class Images2
{
}

public class __invalid_type__17
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images2 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images3
{
}

public class __invalid_type__18
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images3 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images4
{
}

public class __invalid_type__19
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images4 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images5
{
}

public class __invalid_type__20
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images5 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images6
{
    public string @default { get; set; }
}

public class __invalid_type__21
{
    public string art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images6 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images7
{
    public string @default { get; set; }
}

public class __invalid_type__22
{
    public string art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images7 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images8
{
    public string @default { get; set; }
}

public class __invalid_type__23
{
    public string art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images8 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images9
{
}

public class __invalid_type__24
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images9 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images10
{
}

public class __invalid_type__25
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images10 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images11
{
}

public class __invalid_type__26
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images11 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images12
{
}

public class __invalid_type__27
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images12 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images13
{
    public string @default { get; set; }
}

public class __invalid_type__37
{
    public string art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images13 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images14
{
}

public class __invalid_type__38
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images14 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images15
{
}

public class __invalid_type__39
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images15 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images16
{
    public string @default { get; set; }
}

public class __invalid_type__40
{
    public string art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images16 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images17
{
    public string @default { get; set; }
}

public class __invalid_type__41
{
    public string art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images17 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images18
{
}

public class __invalid_type__42
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images18 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images19
{
}

public class __invalid_type__43
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images19 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images20
{
    public string @default { get; set; }
}

public class __invalid_type__46
{
    public string art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images20 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images21
{
}

public class __invalid_type__48
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images21 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images22
{
}

public class __invalid_type__49
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images22 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images23
{
    public string @default { get; set; }
}

public class __invalid_type__50
{
    public string art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images23 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images24
{
}

public class __invalid_type__51
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images24 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images25
{
}

public class __invalid_type__52
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images25 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images26
{
}

public class __invalid_type__54
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images26 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images27
{
}

public class __invalid_type__55
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images27 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images28
{
    public string @default { get; set; }
}

public class __invalid_type__61
{
    public string art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images28 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images29
{
}

public class __invalid_type__62
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images29 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images30
{
}

public class __invalid_type__71
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images30 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images31
{
}

public class __invalid_type__72
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images31 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images32
{
    public string @default { get; set; }
}

public class __invalid_type__93
{
    public string art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images32 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images33
{
}

public class __invalid_type__94
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images33 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images34
{
}

public class __invalid_type__107
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images34 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images35
{
    public string @default { get; set; }
}

public class __invalid_type__108
{
    public string art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images35 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images36
{
    public string @default { get; set; }
}

public class __invalid_type__109
{
    public string art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images36 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images37
{
    public string @default { get; set; }
}

public class __invalid_type__110
{
    public string art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images37 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images38
{
    public string @default { get; set; }
}

public class __invalid_type__111
{
    public string art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images38 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images39
{
}

public class __invalid_type__118
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images39 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images40
{
}

public class __invalid_type__119
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images40 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images41
{
}

public class __invalid_type__120
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images41 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images42
{
}

public class __invalid_type__121
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images42 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images43
{
}

public class __invalid_type__127
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images43 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images44
{
}

public class __invalid_type__128
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images44 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images45
{
}

public class __invalid_type__129
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images45 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images46
{
    public string @default { get; set; }
}

public class __invalid_type__130
{
    public string art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images46 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images47
{
}

public class __invalid_type__131
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images47 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images48
{
    public string @default { get; set; }
}

public class __invalid_type__132
{
    public string art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images48 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images49
{
}

public class __invalid_type__138
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images49 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images50
{
    public string @default { get; set; }
}

public class __invalid_type__139
{
    public string art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images50 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images51
{
}

public class __invalid_type__140
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images51 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images52
{
    public string @default { get; set; }
}

public class __invalid_type__141
{
    public string art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images52 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images53
{
}

public class __invalid_type__173
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images53 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images54
{
}

public class __invalid_type__179
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images54 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images55
{
}

public class __invalid_type__186
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images55 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images56
{
}

public class __invalid_type__187
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images56 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images57
{
}

public class __invalid_type__188
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images57 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images58
{
}

public class __invalid_type__197
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images58 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images59
{
}

public class __invalid_type__202
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images59 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images60
{
}

public class __invalid_type__204
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images60 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images61
{
}

public class __invalid_type__205
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images61 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images62
{
}

public class __invalid_type__207
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images62 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images63
{
    public string @default { get; set; }
}

public class __invalid_type__214
{
    public string art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images63 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images64
{
}

public class __invalid_type__227
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images64 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images65
{
    public string @default { get; set; }
}

public class __invalid_type__228
{
    public string art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images65 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images66
{
    public string @default { get; set; }
}

public class __invalid_type__229
{
    public string art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images66 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images67
{
    public string @default { get; set; }
}

public class __invalid_type__273
{
    public string art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images67 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images68
{
    public string @default { get; set; }
}

public class __invalid_type__274
{
    public string art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images68 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images69
{
}

public class __invalid_type__282
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images69 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images70
{
}

public class __invalid_type__297
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images70 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images71
{
    public string @default { get; set; }
}

public class __invalid_type__298
{
    public string art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images71 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images72
{
    public string @default { get; set; }
}

public class __invalid_type__299
{
    public string art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images72 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images73
{
}

public class __invalid_type__303
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images73 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images74
{
}

public class __invalid_type__304
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images74 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images75
{
}

public class __invalid_type__305
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images75 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images76
{
    public string @default { get; set; }
}

public class __invalid_type__306
{
    public string art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images76 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images77
{
    public string @default { get; set; }
}

public class __invalid_type__307
{
    public string art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images77 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images78
{
    public string @default { get; set; }
}

public class __invalid_type__322
{
    public string art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images78 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images79
{
    public string @default { get; set; }
}

public class __invalid_type__329
{
    public string art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images79 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images80
{
    public string @default { get; set; }
}

public class __invalid_type__330
{
    public string art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images80 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images81
{
}

public class __invalid_type__331
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images81 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images82
{
}

public class __invalid_type__332
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images82 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images83
{
}

public class __invalid_type__333
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images83 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images84
{
}

public class __invalid_type__334
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images84 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images85
{
    public string @default { get; set; }
}

public class __invalid_type__335
{
    public string art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images85 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images86
{
}

public class __invalid_type__336
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images86 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images87
{
}

public class __invalid_type__337
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images87 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public object release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images88
{
    public string @default { get; set; }
}

public class __invalid_type__342
{
    public string art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images88 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images89
{
}

public class __invalid_type__343
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images89 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images90
{
    public string @default { get; set; }
}

public class __invalid_type__344
{
    public string art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images90 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class Images91
{
}

public class __invalid_type__345
{
    public object art_url { get; set; }
    public string artist { get; set; }
    public int channel_id { get; set; }
    public int duration { get; set; }
    public Images91 images { get; set; }
    public int length { get; set; }
    public int network_id { get; set; }
    public string release { get; set; }
    public int started { get; set; }
    public string title { get; set; }
    public string track { get; set; }
    public int track_id { get; set; }
    public string type { get; set; }
}

public class TrackHistory
{
    public __invalid_type__17 __invalid_name__17 { get; set; }
    public __invalid_type__18 __invalid_name__18 { get; set; }
    public __invalid_type__19 __invalid_name__19 { get; set; }
    public __invalid_type__20 __invalid_name__20 { get; set; }
    public __invalid_type__21 __invalid_name__21 { get; set; }
    public __invalid_type__22 __invalid_name__22 { get; set; }
    public __invalid_type__23 __invalid_name__23 { get; set; }
    public __invalid_type__24 __invalid_name__24 { get; set; }
    public __invalid_type__25 __invalid_name__25 { get; set; }
    public __invalid_type__26 __invalid_name__26 { get; set; }
    public __invalid_type__27 __invalid_name__27 { get; set; }
    public __invalid_type__37 __invalid_name__37 { get; set; }
    public __invalid_type__38 __invalid_name__38 { get; set; }
    public __invalid_type__39 __invalid_name__39 { get; set; }
    public __invalid_type__40 __invalid_name__40 { get; set; }
    public __invalid_type__41 __invalid_name__41 { get; set; }
    public __invalid_type__42 __invalid_name__42 { get; set; }
    public __invalid_type__43 __invalid_name__43 { get; set; }
    public __invalid_type__46 __invalid_name__46 { get; set; }
    public __invalid_type__48 __invalid_name__48 { get; set; }
    public __invalid_type__49 __invalid_name__49 { get; set; }
    public __invalid_type__50 __invalid_name__50 { get; set; }
    public __invalid_type__51 __invalid_name__51 { get; set; }
    public __invalid_type__52 __invalid_name__52 { get; set; }
    public __invalid_type__54 __invalid_name__54 { get; set; }
    public __invalid_type__55 __invalid_name__55 { get; set; }
    public __invalid_type__61 __invalid_name__61 { get; set; }
    public __invalid_type__62 __invalid_name__62 { get; set; }
    public __invalid_type__71 __invalid_name__71 { get; set; }
    public __invalid_type__72 __invalid_name__72 { get; set; }
    public __invalid_type__93 __invalid_name__93 { get; set; }
    public __invalid_type__94 __invalid_name__94 { get; set; }
    public __invalid_type__107 __invalid_name__107 { get; set; }
    public __invalid_type__108 __invalid_name__108 { get; set; }
    public __invalid_type__109 __invalid_name__109 { get; set; }
    public __invalid_type__110 __invalid_name__110 { get; set; }
    public __invalid_type__111 __invalid_name__111 { get; set; }
    public __invalid_type__118 __invalid_name__118 { get; set; }
    public __invalid_type__119 __invalid_name__119 { get; set; }
    public __invalid_type__120 __invalid_name__120 { get; set; }
    public __invalid_type__121 __invalid_name__121 { get; set; }
    public __invalid_type__127 __invalid_name__127 { get; set; }
    public __invalid_type__128 __invalid_name__128 { get; set; }
    public __invalid_type__129 __invalid_name__129 { get; set; }
    public __invalid_type__130 __invalid_name__130 { get; set; }
    public __invalid_type__131 __invalid_name__131 { get; set; }
    public __invalid_type__132 __invalid_name__132 { get; set; }
    public __invalid_type__138 __invalid_name__138 { get; set; }
    public __invalid_type__139 __invalid_name__139 { get; set; }
    public __invalid_type__140 __invalid_name__140 { get; set; }
    public __invalid_type__141 __invalid_name__141 { get; set; }
    public __invalid_type__173 __invalid_name__173 { get; set; }
    public __invalid_type__179 __invalid_name__179 { get; set; }
    public __invalid_type__186 __invalid_name__186 { get; set; }
    public __invalid_type__187 __invalid_name__187 { get; set; }
    public __invalid_type__188 __invalid_name__188 { get; set; }
    public __invalid_type__197 __invalid_name__197 { get; set; }
    public __invalid_type__202 __invalid_name__202 { get; set; }
    public __invalid_type__204 __invalid_name__204 { get; set; }
    public __invalid_type__205 __invalid_name__205 { get; set; }
    public __invalid_type__207 __invalid_name__207 { get; set; }
    public __invalid_type__214 __invalid_name__214 { get; set; }
    public __invalid_type__227 __invalid_name__227 { get; set; }
    public __invalid_type__228 __invalid_name__228 { get; set; }
    public __invalid_type__229 __invalid_name__229 { get; set; }
    public __invalid_type__273 __invalid_name__273 { get; set; }
    public __invalid_type__274 __invalid_name__274 { get; set; }
    public __invalid_type__282 __invalid_name__282 { get; set; }
    public __invalid_type__297 __invalid_name__297 { get; set; }
    public __invalid_type__298 __invalid_name__298 { get; set; }
    public __invalid_type__299 __invalid_name__299 { get; set; }
    public __invalid_type__303 __invalid_name__303 { get; set; }
    public __invalid_type__304 __invalid_name__304 { get; set; }
    public __invalid_type__305 __invalid_name__305 { get; set; }
    public __invalid_type__306 __invalid_name__306 { get; set; }
    public __invalid_type__307 __invalid_name__307 { get; set; }
    public __invalid_type__322 __invalid_name__322 { get; set; }
    public __invalid_type__329 __invalid_name__329 { get; set; }
    public __invalid_type__330 __invalid_name__330 { get; set; }
    public __invalid_type__331 __invalid_name__331 { get; set; }
    public __invalid_type__332 __invalid_name__332 { get; set; }
    public __invalid_type__333 __invalid_name__333 { get; set; }
    public __invalid_type__334 __invalid_name__334 { get; set; }
    public __invalid_type__335 __invalid_name__335 { get; set; }
    public __invalid_type__336 __invalid_name__336 { get; set; }
    public __invalid_type__337 __invalid_name__337 { get; set; }
    public __invalid_type__342 __invalid_name__342 { get; set; }
    public __invalid_type__343 __invalid_name__343 { get; set; }
    public __invalid_type__344 __invalid_name__344 { get; set; }
    public __invalid_type__345 __invalid_name__345 { get; set; }
}

public class RootObject
{
    public string ad_network { get; set; }
    public List<object> ad_networks { get; set; }
    public List<Asset> assets { get; set; }
    public string cached_at { get; set; }
    public List<ChannelFilter> channel_filters { get; set; }
    public List<object> events { get; set; }
    public object notification { get; set; }
    public List<StreamSet> stream_sets { get; set; }
    public TrackHistory track_history { get; set; }
}
if FORMAT:match 'latex' then
    function Cite(elem)
        -- print(elem)
        -- print(pandoc.utils.stringify(elem))
        citations = {}
        for i, citet in ipairs(elem.citations) do
            prefix = pandoc.utils.stringify(citet.prefix)
            if prefix ~= '' then
                prefix = prefix .. ' '
            end
            suffix = pandoc.utils.stringify(citet.suffix)
            if suffix ~= '' then
                suffix = ' ' .. suffix
            end

            if citet.mode == 'AuthorInText' then
                citation_text = '\\citet{' .. citet.id .. '}'
            end
            if citet.mode == 'SuppressAuthor' then
                citation_text = '\\citeyear{' .. citet.id .. '}'
            end
            if citet.mode == 'NormalCitation' then
                citation_text = '\\cite{' .. citet.id .. '}'
            end

            if i ~= 1 then
                prefix = ' ' .. prefix
            end
            table.insert(
                citations,
                pandoc.RawInline('latex', prefix .. citation_text .. suffix)
            )
        end
        return citations
    end
end
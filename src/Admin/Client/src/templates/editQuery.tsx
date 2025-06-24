import React, { useState } from 'react';
import {
    ActionMenu,
    ActionMenuHeadline,
    BarItem,
    Button,
    ButtonColor,
    ButtonSize,
    Card,
    Cols,
    Column,
    MenuItem,
    Row,
    Spacing,
    Stack,
    TextArea
} from '@kentico/xperience-admin-components';
import { usePageCommand } from '@kentico/xperience-admin-base';

/** Copy of C# DatabaseTable class */
interface DatabaseTable {
    name: string;
    columns: string[];
}

/** Copy of C# SavedQuery class */
interface SavedQuery {
    id: number;
    name: string;
    text: string;
}

/** Copy of C# EditSqlTemplateClientProperties class */
interface EditQueryClientProperties {
    tables: DatabaseTable[];
    query: string | undefined;
    savedQueries: SavedQuery[];
}

export const EditQueryTemplate = (props: EditQueryClientProperties) => {
    const textAreaRef = React.createRef<HTMLTextAreaElement>();
    const [queryText, setQueryText] = useState(props.query);
    const { execute: runSql } = usePageCommand<void, string>("RunSql");
    const { execute: notify } = usePageCommand<void, string>("Notify");
    const { execute: saveQuery } = usePageCommand<void, string[]>("SaveQuery");
    const { execute: deleteQuery } = usePageCommand<void, number>("DeleteQuery");

    const renderSavedQueries = () => {
        return props.savedQueries.map(q =>
            <BarItem
                leadingButtons={[
                    {
                        label: 'Copy',
                        icon: 'xp-doc-copy',
                        tooltip: 'Copy query to text box',
                        onClick: () => transferClick(q.text)
                    },
                    {
                        label: 'Delete',
                        icon: 'xp-bin',
                        tooltip: 'Delete',
                        onClick: () => deleteClick(q.id)
                    }
                ]}
                headerColumns={[
                    {
                        content: <span>{q.name}</span>
                    }
                ]}>
                <span>{q.text}</span>
            </BarItem>
        );
    };

    /**
     * Click handler to delete a saved query.
     */
    const deleteClick = (id: number) => {
        var confirmed = confirm('Are you sure you want to delete this query?');
        if (confirmed) {
            deleteQuery(id);
        }
    };

    /**
     * Click handler to transfer the text of a saved query to the text box.
     */
    const transferClick = (text: string) => {
        if (!textAreaRef.current) {
            return;
        }

        setQueryText(text);
        textAreaRef.current.textContent = text;
        notify('Query copied to editor');
    };

    /**
     * Click handler for table name in listing to generate SELECT query.
     */
    const generateQuery = (table: DatabaseTable) => {
        if (!textAreaRef.current) {
            return;
        }

        const q = `SELECT ${table.columns.join(', ')} FROM ${table.name}`;
        setQueryText(q);
        textAreaRef.current.textContent = q;
        notify('SQL query generated');
    };

    /**
     * Click handler to execute the current text.
     */
    const runClick = () => {
        if (queryText && queryText.length > 0) {
            runSql(queryText);
        }
        else {
            alert('Please enter a query to execute.');
        }
    };

    /**
     * Click handler for copying the current text to clipboard.
     */
    const copyClick = async () => {
        if (!textAreaRef.current?.textContent) {
            return;
        }

        await navigator.clipboard.writeText(textAreaRef.current.textContent);
        notify('Query copied to clipboard');
    };

    /**
     * Click handler for saving the current text as new SQL query.
     */
    const saveClick = async () => {
        if (!textAreaRef.current?.textContent) {
            return;
        }

        const name = prompt('Enter new query name');
        if (!name) {
            return;
        }

        saveQuery([name, textAreaRef.current.textContent]);
    };

    return (
        <Stack spacing={Spacing.XL}>
            <Button label='Run' color={ButtonColor.Primary} onClick={runClick} /><br/>

            <Row spacing={Spacing.XL}>
                <Column cols={Cols.Col10}>
                    <Stack spacing={Spacing.XL}>
                        <Card headline='Query'>
                            <TextArea
                                minRows={10}
                                maxRows={40}
                                value={queryText}
                                textAreaRef={textAreaRef}
                                placeholder='Enter SQL query...'
                                onChange={(e) => setQueryText(e.target.value)}
                                renderActions={() => (
                                    <>
                                        <Button label='Copy' size={ButtonSize.S} onClick={copyClick} icon='xp-doc-copy' />
                                        <Button label='Save' size={ButtonSize.S} onClick={saveClick} icon='xp-doc-plus' />
                                    </>
                                )} />
                        </Card>
                        {props.savedQueries.length > 0 &&
                            <Card headline='Saved queries'>
                                {renderSavedQueries()}
                            </Card>
                        }
                    </Stack>
                </Column>

                {(props.tables.length > 0) &&
                    <Column cols={Cols.Col2}>
                        <ActionMenu>
                            <ActionMenuHeadline label='Tables' />
                            {
                                props.tables.map(table =>
                                    <MenuItem primaryLabel={table.name} onClick={() => generateQuery(table)} />
                                )
                            }
                        </ActionMenu>
                    </Column>
                }
            </Row>
        </Stack>
    );
};
